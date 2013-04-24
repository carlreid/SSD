using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SSD
{
    class OptionsMenu : Menu
    {
        public OptionsMenu(Game1 mainGame, Menu callerMenu, Viewport viewport, int? selectedItem = null, Vector2? menuOffset = null) :
            base(mainGame, callerMenu, viewport, 1)
        {
            _menuItems.Add("Easy");
            _menuItems.Add("Medium");
            _menuItems.Add("Hard");
            if (menuOffset.HasValue)
            {
                _menuOffset = (Vector2)menuOffset;
            }
            else
            {
                _menuOffset.X = viewport.Width / 2;
                _menuOffset.Y = (viewport.Height / 2) - 100;
            }
        }

        override protected void handleSelected(string item, ref Menu currentDisplay, ref bool inMenu)
        {
            switch(item){
                case "Easy":
                    currentDisplay = _callerMenu;
                    _mainGame.setGameDifficulty(0);
                    break;
                case "Medium":
                    currentDisplay = _callerMenu;
                    _mainGame.setGameDifficulty(1);
                    break;
                case "Hard":
                    currentDisplay = _callerMenu;
                    _mainGame.setGameDifficulty(2);
                    break;
                default:
                    break;
            }
        }

        protected override void handleBack(string item, ref Menu currentDisplay, ref bool inMenu)
        {
            currentDisplay = _callerMenu;
        }
    }
}
