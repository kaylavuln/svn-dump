﻿package mx.managers
{
    import flash.events.*;

    public interface IBrowserManager extends IEventDispatcher
    {

        public function IBrowserManager();

        function get fragment() : String;

        function get base() : String;

        function setFragment(param1:String) : void;

        function setTitle(param1:String) : void;

        function init(param1:String = null, param2:String = null) : void;

        function get title() : String;

        function initForHistoryManager() : void;

        function get url() : String;

    }
}
