using System.Diagnostics;
using System.Windows.Forms;

namespace Uzhik
{
    public partial class View : Form
    {
        private Model _model;
        private Controller _controller;
        public const int CellSize = 15; // ������ ������
        private int score = 0; // ���� ��� ������������ �����
        private Label scoreLabel;

        public View()
        {
            InitializeComponent();
            InitializeGame();
            InitializeScoreLabel(); // �������� ����� ������������� �������� ���������� ��� ����������� �����
            // �������������� ����������
            _controller = new Controller(this, _model);
            // ������������� �� ������� KeyDown
            this.KeyDown += new KeyEventHandler(View_KeyDown);
            this.DoubleBuffered = true; // ��������� ������� �����������
        }

        private void InitializeGame()
        {
            _model = new Model(this.ClientSize.Width, this.ClientSize.Height); // �������� ���������� Model � ��������� ������ � ������ �����
            // ��������� ��������� ������� ��� ���������� ����
            timer1.Interval = 100; // ������� ������ �������� ���������� (��������, 100 �����������)
            timer1.Start();
        }

        private void InitializeScoreLabel()
        {
            scoreLabel = new Label();
            //scoreLabel.Text = "Score: "; // ������������� ��������� �������� �����
            scoreLabel.AutoSize = true;
            scoreLabel.Location = new Point(10, 10); // ������������� ������� �������� �� �����
            Controls.Add(scoreLabel); // ��������� ������� �� �����
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            /*_model.Move(pictureBox1.Width / CellSize, pictureBox1.Height / CellSize);
            pictureBox1.Invalidate(); // ���������� PictureBox*/
            _model.Move();
            if (_model._food == null)
            {
                // ���� ���, ���������� ����� �������������� ���
                _model.PlaceFood();
            }
            Point foodPosition = _model._food;
            score = _model.Score; // ��������� �������� �����
            Invalidate(); // ���������� �����
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // ��������� ������
            // ������� ��� ������ ��������, �� � �����-�� ������ ����� �������� ����� ������ ���������, ����� ���������� ����� �������� ����
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

            // ��������� ���
            if (_model._food != null)
            {
                Image foodImage = Properties.Resources.apple;
                e.Graphics.DrawImage(foodImage, _model._food.X * CellSize, _model._food.Y * CellSize, CellSize, CellSize);
                // ��������� ���������� ��������
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