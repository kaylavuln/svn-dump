﻿package 
{
    import GUI.Components.*;
    import flash.display.*;
    import mx.binding.*;
    import mx.core.*;

    public class _GUI_Components_BuildingsListWatcherSetupUtil extends Sprite implements IWatcherSetupUtil
    {

        public function _GUI_Components_BuildingsListWatcherSetupUtil()
        {
            return;
        }// end function

        public function setup(param1:Object, param2:Function, param3:Array, param4:Array) : void
        {
            param4[0] = new PropertyWatcher("dataProvider", {propertyChange:true}, [param3[0]], param2);
            param4[0].updateParent(param1);
            return;
        }// end function

        public static function init(param1:IFlexModuleFactory) : void
        {
            BuildingsList.watcherSetupUtil = new _GUI_Components_BuildingsListWatcherSetupUtil;
            return;
        }// end function

    }
}
