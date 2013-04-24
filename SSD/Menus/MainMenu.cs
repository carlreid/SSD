using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class MainMenu : Menu
    {
        public MainMenu(Game1 mainGame, Menu callerMenu, Viewport viewport, int? selectedItem = null, Vector2? menuOffset = null) :
            base(mainGame, callerMenu, viewport, selectedItem)
        {
            _menuItems.Add("Start Game");
            _menuItems.Add("Options");
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
                case "Start Game":
                    inMenu = false;
                    _mainGame.restartGame(1);
                    break;
                case "Options":
                    currentDisplay = new OptionsMenu(_mainGame, this, _viewport);
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
             //DO NOTHING
        }

    }
}
