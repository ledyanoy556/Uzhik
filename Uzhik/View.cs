using System.Diagnostics;
using System.Windows.Forms;

namespace Uzhik
{
    public partial class View : Form
    {
        private Model _model;
        private Controller _controller;
        public const int CellSize = 15; // Размер клетки
        private int score = 0; // Поле для отслеживания счета
        private Label scoreLabel;

        public View()
        {
            InitializeComponent();
            InitializeGame();
            InitializeScoreLabel(); // Вызываем метод инициализации элемента управления для отображения счета
            // Инициализируем контроллер
            _controller = new Controller(this, _model);
            // Подписываемся на событие KeyDown
            this.KeyDown += new KeyEventHandler(View_KeyDown);
            this.DoubleBuffered = true; // Включение двойной буферизации
        }

        private void InitializeGame()
        {
            _model = new Model(this.ClientSize.Width, this.ClientSize.Height); // Создание экземпляра Model с указанием ширины и высоты формы
            // Установка интервала таймера для обновления игры
            timer1.Interval = 100; // Указать нужный интервал обновления (например, 100 миллисекунд)
            timer1.Start();
        }

        private void InitializeScoreLabel()
        {
            scoreLabel = new Label();
            //scoreLabel.Text = "Score: "; // Устанавливаем начальное значение счета
            scoreLabel.AutoSize = true;
            scoreLabel.Location = new Point(10, 10); // Устанавливаем позицию элемента на форме
            Controls.Add(scoreLabel); // Добавляем элемент на форму
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            /*_model.Move(pictureBox1.Width / CellSize, pictureBox1.Height / CellSize);
            pictureBox1.Invalidate(); // Обновление PictureBox*/
            _model.Move();
            if (_model._food == null)
            {
                // Если нет, генерируем новое местоположение еды
                _model.PlaceFood();
            }
            Point foodPosition = _model._food;
            score = _model.Score; // Обновляем значение счета
            Invalidate(); // Обновление формы
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Отрисовка змейки
            // Пытался при помощи картинок, но в какой-то момент форма начинает очень сильно тормозить, когда появляется много кусочков змеи
            /*Image snakeSegmentImage = Properties.Resources.snake;
            for (int i = 0; i < _model._body.Count(); i++)
            {
                Point point = _model._body.GetSegment(i);
                e.Graphics.DrawImage(snakeSegmentImage, point.X * CellSize, point.Y * CellSize, CellSize, CellSize);
            }*/
            for (int i = 0; i < _model._body.Count(); i++)
            {
                Point point = _model._body.GetSegment(i);
                e.Graphics.FillRectangle(Brushes.Green, point.X * CellSize, point.Y * CellSize, CellSize, CellSize);
            }

            // Отрисовка еды
            if (_model._food != null)
            {
                Image foodImage = Properties.Resources.apple;
                e.Graphics.DrawImage(foodImage, _model._food.X * CellSize, _model._food.Y * CellSize, CellSize, CellSize);
                // Отрисовка встроенной графикой
                //e.Graphics.FillEllipse(Brushes.Red, _model._food.X * CellSize, _model._food.Y * CellSize, CellSize, CellSize);
            }


            string scoreText = "Score: " + score;
            e.Graphics.DrawString(scoreText, Font, Brushes.Black, new PointF(10, 10));

        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.KeyCode.ToString());
            _controller.HandleInput(sender, e);
        }
    }
}