/*
 * Copyright (c) 2024 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

#if DK_VRCSDK3A
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    internal class VRCFourAxisItemWrapper : FourAxisItem
    {
        public override string Name { get => _control.name; set => _control.name = value; }
        public override Texture2D Icon { get => _control.icon; set => _control.icon = value; }
        public override MenuItemController ControllerOnOpen { get => _controllerOnOpenWrapper; set => VRCMenuUtils.WriteMenuItemController(value, _controllerOnOpenWrapper); }

        public override MenuItemController UpController { get => _upControllerWrapper; set => VRCMenuUtils.WriteMenuItemController(value, _upControllerWrapper); }
        public override MenuItemController RightController { get => _rightControllerWrapper; set => VRCMenuUtils.WriteMenuItemController(value, _rightControllerWrapper); }
        public override MenuItemController DownController { get => _downControllerWrapper; set => VRCMenuUtils.WriteMenuItemController(value, _downControllerWrapper); }
        public override MenuItemController LeftController { get => _leftControllerWrapper; set => VRCMenuUtils.WriteMenuItemController(value, _leftControllerWrapper); }

        public override Label UpLabel { get => _upLabelWrapper; set => VRCMenuUtils.WriteLabel(value, _upLabelWrapper); }
        public override Label RightLabel { get => _rightLabelWrapper; set => VRCMenuUtils.WriteLabel(value, _rightLabelWrapper); }
        public override Label DownLabel { get => _downLabelWrapper; set => VRCMenuUtils.WriteLabel(value, _downLabelWrapper); }
        public override Label LeftLabel { get => _leftLabelWrapper; set => VRCMenuUtils.WriteLabel(value, _leftLabelWrapper); }

        private readonly VRCExpressionsMenu.Control _control;
        private readonly VRCAnimatorParameterControllerWrapper _controllerOnOpenWrapper;
        private readonly VRCAnimatorParameterControllerWrapper _upControllerWrapper;
        private readonly VRCAnimatorParameterControllerWrapper _rightControllerWrapper;
        private readonly VRCAnimatorParameterControllerWrapper _downControllerWrapper;
        private readonly VRCAnimatorParameterControllerWrapper _leftControllerWrapper;
        private readonly VRCMenuItemLabelWrapper _upLabelWrapper;
        private readonly VRCMenuItemLabelWrapper _rightLabelWrapper;
        private readonly VRCMenuItemLabelWrapper _downLabelWrapper;
        private readonly VRCMenuItemLabelWrapper _leftLabelWrapper;

        public VRCFourAxisItemWrapper(VRCExpressionsMenu.Control control)
        {
            _control = control;
            _controllerOnOpenWrapper = new VRCAnimatorParameterControllerWrapper(control.parameter, () => control.value, val => control.value = val);

            if (_control.subParameters.Length < 4)
            {
                _control.subParameters = new VRCExpressionsMenu.Control.Parameter[] {
                    new VRCExpressionsMenu.Control.Parameter(),
                    new VRCExpressionsMenu.Control.Parameter(),
                    new VRCExpressionsMenu.Control.Parameter(),
                    new VRCExpressionsMenu.Control.Parameter()
                };
            }

            _upControllerWrapper = new VRCAnimatorParameterControllerWrapper(control.subParameters[0], () => 0.0f, _ => { });
            _rightControllerWrapper = new VRCAnimatorParameterControllerWrapper(control.subParameters[1], () => 0.0f, _ => { });
            _downControllerWrapper = new VRCAnimatorParameterControllerWrapper(control.subParameters[2], () => 0.0f, _ => { });
            _leftControllerWrapper = new VRCAnimatorParameterControllerWrapper(control.subParameters[3], () => 0.0f, _ => { });

            if (_control.labels.Length < 4)
            {
                _control.labels = new VRCExpressionsMenu.Control.Label[] {
                    new VRCExpressionsMenu.Control.Label(),
                    new VRCExpressionsMenu.Control.Label(),
                    new VRCExpressionsMenu.Control.Label(),
                    new VRCExpressionsMenu.Control.Label()
                };
            }

            _upLabelWrapper = new VRCMenuItemLabelWrapper(() => _control.labels[0], val => _control.labels[0] = val);
            _rightLabelWrapper = new VRCMenuItemLabelWrapper(() => _control.labels[1], val => _control.labels[1] = val);
            _downLabelWrapper = new VRCMenuItemLabelWrapper(() => _control.labels[2], val => _control.labels[2] = val);
            _leftLabelWrapper = new VRCMenuItemLabelWrapper(() => _control.labels[3], val => _control.labels[3] = val);
        }
    }
}
#endif
