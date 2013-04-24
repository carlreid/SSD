using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class WinMenu : Menu
    {
        public WinMenu(Game1 mainGame, Menu callerMenu, Viewport viewport, int? selectedItem = null, Vector2? menuOffset = null) :
            base(mainGame, callerMenu, viewport, selectedItem)
        {
            _menuItems.Add("Play Again");
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
            switch (item)
            {
                case "Play Again":
                    _mainGame.restartGame(1);
                    inMenu = false;
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
            currentDisplay = _callerMenu;
            _mainGame.restartGame(1);
        }

    }
}
