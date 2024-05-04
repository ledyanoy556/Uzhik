using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uzhik
{
    public class Controller
    {
        private Model _model;
        private View _view;

        public Controller(View view, Model model)
        {
            _view = view;
            _model = model;
            _view.KeyDown += HandleInput; // Привязываем обработчик нажатий клавиш к событию KeyDown формы
        }

        public void HandleInput(object sender, KeyEventArgs e)
        {
            // Обработка нажатий клавиш пользователем
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _model.TurnLeft();
                    break;
                case Keys.Right:
                    _model.TurnRight();
                    break;
            }
        }
    }

}
