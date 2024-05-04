using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Uzhik
{
    public class Model
    {
        private int _score; // Поле для хранения счета
        public int Score => _score; // Свойство для доступа к счету
        private bool _isMoving = true;
        private double _currentAngle; // Поле для хранения текущего угла направления
        private const int InitialSnakeLength = 3;
        public Point _food { get; private set; } = new Point(-1, -1);// Поле для хранения координат еды
        private readonly int _fieldWidth;
        private readonly int _fieldHeight;
        public Body _body { get; private set; }
        private bool _gameOverShown = false; // Переменная для отслеживания показа окна "Вы проиграли!"// Свойство для хранения координат еды
        private System.Windows.Forms.Timer _timer;

        public Model(int fieldWidth, int fieldHeight)
        {
            _body = new Body();
            _currentAngle = 0; // Инициализация угла направления (0 градусов)
            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;
            _score = 0; // Начальное значение счета

            // Инициализация змейки
            int startX = fieldWidth / 20; // начальная позиция по X (центр поля)
            int startY = fieldHeight / 20; // начальная позиция по Y (центр поля)
            for (int i = 0; i < InitialSnakeLength; i++)
            {
                _body.AddSegment(new Point(startX - i, startY));
            }
        }

        public void Move()
        {
            if (!_isMoving)
            {
                return; // Если нет, то выходим из метода без выполнения движения
            }
            // Получаем текущее положение головы змейки
            Point currentHeadPosition = _body.First();

            // Вычисляем следующее положение головы змейки
            Point nextHeadPosition = GetNextHeadPosition(currentHeadPosition);

            // Добавляем новую позицию головы в начало тела змейки
            _body.Insert(0, nextHeadPosition);

            // Удаляем последний сегмент змейки (хвост), чтобы змейка не росла
            _body.RemoveAt(_body.Count() - 1);

            // Проверка на столкновение с границами экрана
            if (!_gameOverShown && (nextHeadPosition.X < 0 || nextHeadPosition.X >= _fieldWidth/View.CellSize ||
                nextHeadPosition.Y < 0 || nextHeadPosition.Y >= _fieldHeight/View.CellSize))
            {
                _gameOverShown = true;
                StopMoving();
                MessageBox.Show("Вы проиграли!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Закрываем форму после закрытия MessageBox
                Application.Exit(); // Можно также использовать this.Close() если требуется закрыть только текущую форму
                // Здесь можно добавить логику для завершения игры или другие действия
                return;
            }

            // Проверяем столкновение с самой собой
            for (int i = 1; i < _body.Count(); i++)
            {
                if (!_gameOverShown && nextHeadPosition == _body.GetSegment(i))
                {
                    _gameOverShown = true;
                    StopMoving();
                    MessageBox.Show("Вы проиграли!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Закрываем форму после закрытия MessageBox
                    Application.Exit();

                    return;
                }
            }

            // Первая генерация еды
            if (_food.X == -1 && _food.Y == -1)
            {
                PlaceFood();
            }

            // Проверяем столкновение с едой
            CheckCollisionWithFood();
        }

        public class Body
        {
            private List<Point> segments;

            public Body()
            {
                segments = new List<Point>();
            }

            public void AddSegment(Point segment)
            {
                segments.Add(segment);
            }

            public void RemoveLastSegment()
            {
                if (segments.Count > 0)
                {
                    segments.RemoveAt(segments.Count - 1);
                }
            }

            public Point GetSegment(int index)
            {
                if (index >= 0 && index < segments.Count)
                {
                    return segments[index];
                }
                else
                {
                    // Возвращаем пустую точку в случае, если индекс некорректный
                    return new Point(-1, -1);
                }
            }

            public int Count()
            {
                return segments.Count;
            }

            public Point First()
            {
                if (segments.Count > 0)
                {
                    return segments[0];
                }
                else
                {
                    // Возвращаем пустую точку в случае отсутствия сегментов
                    return new Point(-1, -1);
                }
            }

            public void Insert(int index, Point segment)
            {
                if (index >= 0 && index <= segments.Count)
                {
                    segments.Insert(index, segment);
                }
            }

            public void RemoveAt(int index)
            {
                if (index >= 0 && index < segments.Count)
                {
                    segments.RemoveAt(index);
                }
            }

            public bool Contains(Point point)
            {
                return segments.Contains(point);
            }
        }

        public Point GetNextHeadPosition(Point currentHeadPosition)
        {
            // Вычисляем приращение координат головы змейки в зависимости от текущего угла
            double radians = Math.PI * _currentAngle / 180.0; // Переводим угол в радианы
            int dx = (int)Math.Round(Math.Cos(radians)); // Приращение по оси X
            int dy = (int)Math.Round(Math.Sin(radians)); // Приращение по оси Y

            // Вычисляем новые координаты головы змейки
            int newX = currentHeadPosition.X + dx;
            int newY = currentHeadPosition.Y + dy;

            // Создаем и возвращаем новую точку с координатами головы змейки
            return new Point(newX, newY);
        }

        public void TurnRight()
        {
            _currentAngle += 22.5; // Увеличиваем угол на 45 градусов
            /*if (_currentAngle >= 360)
            {
                _currentAngle -= 360;
            }*/
        }

        public void TurnLeft()
        {
            _currentAngle -= 22.5; // Уменьшаем угол на 45 градусов
            /*if (_currentAngle < 0) // Обеспечиваем цикличность угла
            {
                _currentAngle += 360;
            }*/
        }

        public void PlaceFood()
        {
            Random random = new Random();

            // Генерируем случайные координаты для еды
            int foodX = random.Next(0, _fieldWidth / View.CellSize);
            int foodY = random.Next(0, _fieldHeight / View.CellSize);

            // Проверяем, не попадает ли еда на змейку или на уже занятые клетки
            while (_body.Contains(new Point(foodX, foodY)))
            {
                // Если попало, генерируем новые координаты
                foodX = random.Next(0, _fieldWidth / View.CellSize);
                foodY = random.Next(0, _fieldHeight / View.CellSize);
            }

            // Устанавливаем координаты новой еды
            _food = new Point(foodX, foodY);
        }

        public void CheckCollisionWithFood()
        {
            // Проверяем, столкнулась ли змейка с едой
            if (_body.First() == _food)
            {
                // Если змейка съела еду, увеличиваем ее длину вдвое
                _score += 50; // Добавляем 50 очков к счету
                int currentLength = _body.Count();
                for (int i = 0; i < currentLength; i++)
                {
                    _body.AddSegment(_body.GetSegment(i));
                }

                // Перемещаем еду на новое место
                PlaceFood();
            }
        }
        public void StopTimer()
        {
            _timer.Stop();
        }

        // Метод для остановки движения змейки
        public void StopMoving()
        {
            _isMoving = false;
        }

        // Метод для возобновления движения змейки
        public void StartMoving()
        {
            _isMoving = true;
        }

        /*private void Rotate(int degrees)
        {
            // Поворот змейки на указанное количество градусов
            int currentDirection = (int)CurrentDirection;
            currentDirection += degrees / 10; // Учитываем поворот на 10 градусов
            currentDirection %= 36; //36 или 4 хз Обеспечиваем цикличность направлений

            // Преобразуем обратно в enum Direction
            if (currentDirection < 0)
                currentDirection += 36;

            CurrentDirection = (Direction)(currentDirection%36);
        }*/
    }
}
