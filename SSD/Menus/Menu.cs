using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    abstract class Menu
    {
        public Menu(Game1 mainGame, Menu callerMenu, Viewport viewport, int? selectedItem = null){
            _callerMenu = callerMenu;
            _menuItems = new List<string>();
            _selectedItem = selectedItem.HasValue ? (int)selectedItem : 0;
            _menuOffset = Vector2.Zero;
            _viewport = viewport;
            _mainGame = mainGame;
        }

        public void update(GamePadState gamePadState, GamePadState lastGamePadState, KeyboardState keyboardState, KeyboardState lastKeyboardState, ref Menu currentDisplay, ref bool inMenu)
        {
            if (gamePadState.DPad.Up == ButtonState.Pressed && lastGamePadState.DPad.Up == ButtonState.Released
                || keyboardState.IsKeyDown(Keys.W) && lastKeyboardState.IsKeyUp(Keys.W) || keyboardState.IsKeyDown(Keys.Up) && lastKeyboardState.IsKeyUp(Keys.Up))
            {
                goUp();
            }
            else if (gamePadState.DPad.Down == ButtonState.Pressed && lastGamePadState.DPad.Down == ButtonState.Released
                || keyboardState.IsKeyDown(Keys.S) && lastKeyboardState.IsKeyUp(Keys.S) || keyboardState.IsKeyDown(Keys.Down) && lastKeyboardState.IsKeyUp(Keys.Down))
            {
                goDown();
            }
            else if (gamePadState.Buttons.A == ButtonState.Pressed && lastGamePadState.Buttons.A == ButtonState.Released
                || keyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space))
            {
                handleSelected(_menuItems[_selectedItem], ref  currentDisplay, ref inMenu);
            }
            //else
            //{
            //    goDown();
            //}
        }

        private void goUp()
        {
            _selectedItem -= 1;
            if (_selectedItem < 0)
            {
                _selectedItem = _menuItems.Count - 1;
            }
        }

        private void goDown()
        {
            _selectedItem += 1;
            if (_selectedItem >= _menuItems.Count)
            {
                _selectedItem = 0;
            }
        }

        public List<string> getMenuItems()
        {
            return _menuItems;
        }
        public Vector2 getOffset()
        {
            return _menuOffset;
        }

        public int getSelectedItem()
        {
            return _selectedItem;
        }

        virtual protected void handleSelected(string item, ref Menu currentDisplay, ref bool inMenu)
        {
            //Handle in menu class
        }

        protected List<string> _menuItems;
        protected Menu _callerMenu;
        protected int _selectedItem;
        protected Vector2 _menuOffset;
        protected Viewport _viewport;
        protected Game1 _mainGame;
    }
}
