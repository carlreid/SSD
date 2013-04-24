using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SSD
{
    class GameOverMenu : Menu
    {
        public GameOverMenu(Game1 mainGame, Menu callerMenu, Viewport viewport, int? selectedItem = null, Vector2? menuOffset = null) :
            base(mainGame, callerMenu, viewport, selectedItem)
        {
            _menuItems.Add("Try Again");
            _menuItems.Add("Main Menu");
            _menuItems.Add("Exit Game");
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
                case "Try Again":
                    inMenu = false;
                    _mainGame.restartGame(1);
                    break;
                case "Main Menu":
                    currentDisplay = _callerMenu;
                    _mainGame.restartGame(1);
                    break;
                case "Exit Game":
                    _mainGame.Exit();
                    break;
                default:
                    break;
            }
        }

        protected override void handleBack(string item, ref Menu currentDisplay, ref bool inMenu)
        {
            inMenu = false;
            _mainGame.restartGame(1);
        }
    }
}
