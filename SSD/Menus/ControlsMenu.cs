using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SSD
{
    class ControlsMenu : Menu
    {
        public ControlsMenu(Game1 mainGame, Menu callerMenu, Viewport viewport, int? selectedItem = null, Vector2? menuOffset = null) :
            base(mainGame, callerMenu, viewport, 0)
        {
            _menuItems.Add("Back");
            if (menuOffset.HasValue)
            {
                _menuOffset = (Vector2)menuOffset;
            }
            else
            {
                _menuOffset.X = viewport.Width / 2;
                _menuOffset.Y = (viewport.Height / 2) + 200;
            }
        }

        override protected void handleSelected(string item, ref Menu currentDisplay, ref bool inMenu)
        {
            switch(item){
                case "Back":
                    currentDisplay = _callerMenu;
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
