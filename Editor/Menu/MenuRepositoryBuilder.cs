/*
 * Copyright (c) 2023 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

using UnityEngine;

namespace Chocopoi.DressingFramework.Menu
{
    public class MenuRepositoryBuilder
    {
        private readonly IMenuRepository _menu;
        private readonly MenuRepositoryBuilder _parentBuilder;
        private readonly string _parentMenuName;
        private readonly Texture2D _parentMenuIcon;

        private MenuRepositoryBuilder(MenuRepositoryBuilder parentBuilder, string parentMenuName, Texture2D parentMenuIcon)
        {
            _menu = new MenuGroup();
            _parentBuilder = parentBuilder;
            _parentMenuName = parentMenuName;
            _parentMenuIcon = parentMenuIcon;
        }

        public MenuRepositoryBuilder(IMenuRepository menu)
        {
            _menu = menu;
            _parentMenuName = null;
            _parentBuilder = null;
        }

        public MenuRepositoryBuilder AddMenuItem(MenuItem item)
        {
            _menu.Add(item);
            return this;
        }

        public MenuRepositoryBuilder AddButton(string name, MenuItemController controller, Texture2D icon = null)
        {
            _menu.Add(new ButtonItem()
            {
                Name = name,
                Icon = icon,
                Controller = controller
            });
            return this;
        }

        public MenuRepositoryBuilder AddButton(string name, string animatorParameter, float animatorParameterValue, Texture2D icon = null)
        {
            _menu.Add(new ButtonItem()
            {
                Name = name,
                Icon = icon,
                Controller = new AnimatorParameterController()
                {
                    ParameterName = animatorParameter,
                    ParameterValue = animatorParameterValue
                }
            });
            return this;
        }

        public MenuRepositoryBuilder AddToggle(string name, MenuItemController controller, Texture2D icon = null)
        {
            _menu.Add(new ToggleItem()
            {
                Name = name,
                Icon = icon,
                Controller = controller
            });
            return this;
        }

        public MenuRepositoryBuilder AddToggle(string name, string animatorParameter, float animatorParameterValue, Texture2D icon = null)
        {
            _menu.Add(new ToggleItem()
            {
                Name = name,
                Icon = icon,
                Controller = new AnimatorParameterController()
                {
                    ParameterName = animatorParameter,
                    ParameterValue = animatorParameterValue
                }
            });
            return this;
        }

        public MenuRepositoryBuilder AddSubMenu(string name, MenuGroup menuGroup, Texture2D icon = null)
        {
            // TODO: parameter on open
            _menu.Add(new SubMenuItem()
            {
                Name = name,
                Icon = icon,
                SubMenu = menuGroup
            });
            return this;
        }

        public MenuRepositoryBuilder BeginNewSubMenu(string name, Texture2D icon = null)
        {
            return new MenuRepositoryBuilder(this, name, icon);
        }

        public MenuRepositoryBuilder EndNewSubMenu()
        {
            if (_parentBuilder == null)
            {
                throw new System.Exception("There is no parent builder to end! Are you not using together with BeginNewSubMenu()?");
            }
            _parentBuilder.AddSubMenu(_parentMenuName, (MenuGroup)_menu, _parentMenuIcon);
            return _parentBuilder;
        }

        public IMenuRepository GetContainingMenuRepository()
        {
            return _menu;
        }
    }
}
