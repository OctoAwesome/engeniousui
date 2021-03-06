﻿using System.Collections.Generic;
using engenious;
using engenious.Graphics;
using engenious.Input;
using EngeniousUI.Controls;

namespace EngeniousUI
{
    internal abstract class EventArgsPool<T> where T : EventArgs,new()
    {

        private readonly Stack<T> _freeList = new Stack<T>(16);
        private readonly object _lockObj = new object();

        public T Take()
        {
            if (_freeList.Count <= 0) return new T();
            lock (_lockObj)
            {
                if (_freeList.Count > 0)
                {
                    return _freeList.Pop();
                }
            }

            return new T();
        }

        public void Release(T arr)
        {
            if (arr == null) return;
            ResetVariable(arr);

            lock (_lockObj)
            {
                _freeList.Push(arr);
            }
        }

        protected abstract void ResetVariable(T arr);
    }

    /// <summary>
    /// Basisklasse für alle Arten von Event Args innerhalb des UI Frameworks
    /// </summary>
    public class EventArgs
    {
        /// <summary>
        /// Gibt an ob das Event bereits verarbeitet wurde oder legt dies fest.
        /// </summary>
        public bool Handled { get; set; }
    }
    internal class EventArgsPool : EventArgsPool<EventArgs>
    {
        private static EventArgsPool _instance;
        public static EventArgsPool Instance
        {
            get {return _instance = _instance ?? new EventArgsPool(); }
        }
        protected override void ResetVariable(EventArgs arr)
        {
            arr.Handled = false;
        }
    }

    /// <summary>
    /// Basisklasse für alle DragDrop Events
    /// </summary>
    public class DragEventArgs : PointerEventArgs
    {
        /// <summary>
        /// Optionales Feld um das sendende Control einzufügen.
        /// </summary>
        public Control Sender { get; set; }

        /// <summary>
        /// Optionales Icon, das während des Drag-Vorgangs angezeigt werden soll.
        /// </summary>
        public Texture2D Icon { get; set; }

        /// <summary>
        /// Angabe der Größe des Icons, das beim Drag-Vorgang angezeigt wird.
        /// </summary>
        public Point IconSize { get; set; }

        /// <summary>
        /// Content, der gedraggt wird.
        /// </summary>
        public object Content { get; set; }
    }
    internal class DragEventArgsPool : EventArgsPool<DragEventArgs>
    {
        private static DragEventArgsPool _instance;
        public static DragEventArgsPool Instance
        {
            get {return _instance = _instance ?? new DragEventArgsPool(); }
        }
        protected override void ResetVariable(DragEventArgs arr)
        {
            arr.Handled = false;
            arr.Sender = null;
            arr.Icon = null;
            arr.IconSize = Point.Zero;
            arr.Content = null;
        }
    }
    /// <summary>
    /// Basisklasse für alle Positionsbasierten Events (Maus, Touch)
    /// </summary>
    public abstract class PointerEventArgs : EventArgs
    {
        /// <summary>
        /// Gibt an, ob das Event 
        /// </summary>
        public bool Bubbled { get; set; }

        /// <summary>
        /// Position des Mauspointers bezogen auf den Ursprung des aktuellen Controls
        /// </summary>
        public Point LocalPosition { get; set; }

        /// <summary>
        /// Position des Mauspointers in globaler Screen-Koordinate
        /// </summary>
        public Point GlobalPosition { get; set; }
    }

    /// <summary>
    /// Standard Event Args bei Property Changed Events.
    /// </summary>
    /// <typeparam name="T">Typ des Properties</typeparam>
    public class PropertyEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Der alte Wert der Property
        /// </summary>
        public T OldValue { get; set; }

        /// <summary>
        /// Der neue Wert der Property
        /// </summary>
        public T NewValue { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instaz der PropertyEventArgs-Klasse
        /// </summary>
        public PropertyEventArgs() { }

        /// <summary>
        /// Erzeugt eine neue Instaz der PropertyEventArgs-Klasse
        /// </summary>
        /// <param name="oldValue">Der alte Wert</param>
        /// <param name="newValue">Der neue Wert</param>
        public PropertyEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }


    internal class MouseEventArgsPool : EventArgsPool<MouseEventArgs>
    {
        private static MouseEventArgsPool _instance;
        public static MouseEventArgsPool Instance
        {
            get {return _instance = _instance ?? new MouseEventArgsPool(); }
        }
        protected override void ResetVariable(MouseEventArgs arr)
        {
            arr.Handled = false;
            arr.MouseMode = MouseMode.Captured;
            arr.Bubbled = false;
            arr.GlobalPosition = Point.Zero;
            arr.LocalPosition = Point.Zero;
        }
    }
    /// <summary>
    /// Event Arguments für alle Mouse Events.
    /// </summary>
    public class MouseEventArgs : PointerEventArgs
    {
        /// <summary>
        /// Gibt den aktuellen Modus der Maus an.
        /// </summary>
        public MouseMode MouseMode { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instanz der MouseEventArgs-Klasse
        /// </summary>
        public MouseEventArgs() { }

        /// <summary>
        /// Erzeugt eine neue Instanz der MouseEventArgs-Klasse
        /// </summary>
        /// <param name="mouseMode">Der aktuelle Modus der Maus</param>
        /// <param name="localPosition">Position des Mauspointers bezogen auf den Ursprung des aktuellen Controls</param>
        /// <param name="globalPosition">Position des Mauspointers in globaler Screen-Koordinate</param>
        public MouseEventArgs(MouseMode mouseMode, Point localPosition, Point globalPosition)
        {
            MouseMode = mouseMode;
            LocalPosition = localPosition;
            GlobalPosition = globalPosition;
        }
    }

    /// <summary>
    /// Event Arguments für Maus Scroll Events
    /// </summary>
    public class MouseScrollEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Gibt die  Anzahl der gescrollte Einheiten an.
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instanz der MouseScrollEventArgs-Klasse.
        /// </summary>
        public MouseScrollEventArgs() { }

        /// <summary>
        /// Erzeugt eine neue Instanz der MouseScrollEventArgs-Klasse.
        /// </summary>
        /// <param name="steps">Anzahl der gescrollten Einheiten</param>
        public MouseScrollEventArgs(int steps)
        {
            Steps = steps;
        }
    }

    /// <summary>
    /// Event Args für alle Touch-basierten Events.
    /// </summary>
    public class TouchEventArgs : PointerEventArgs
    {
        /// <summary>
        /// ID des Touch Points.
        /// </summary>
        public int TouchId { get; set; }
    }

    internal static class KeyEventArgsPool
    {
        private static readonly Stack<KeyEventArgs> FreeList = new Stack<KeyEventArgs>(16);
        private static readonly object LockObj = new object();
        
        public static KeyEventArgs Take()
        {
            if (FreeList.Count > 0)
            {
                lock (LockObj)
                {
                    if (FreeList.Count > 0)
                    {
                        return FreeList.Pop();
                    }
                }
                
            }

            return new KeyEventArgs();
        }

        public static void Release(KeyEventArgs arr)
        {
            arr.Key = Keys.Unknown;
            arr.Alt = false;
            arr.Shift = false;
            arr.Ctrl = false;
            arr.Handled = false;

            lock (LockObj)
            {
                FreeList.Push(arr);
            }
        }
    }

    /// <summary>
    /// Event Arguemnts für Tastatur-Events.
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        /// <summary>
        /// Gibt an ob eine der Shift-Tasten gedrückt wurde.
        /// </summary>
        public bool Shift { get; set; }

        /// <summary>
        /// Gibt an ob die Steuerungstaste gedrückt wurde.
        /// </summary>
        public bool Ctrl { get; set; }

        /// <summary>
        /// Gibts an ob eine der Alt-Tasten gedrückt wurde.
        /// </summary>
        public bool Alt { get; set; }

        /// <summary>
        /// Gibt die Taste an, für die das Event gefeuert wurde.
        /// </summary>
        public Keys Key { get; set; }
    }

    /// <summary>
    /// Event Arguments für Text-Eingabe
    /// </summary>
    public class KeyTextEventArgs : EventArgs
    {
        /// <summary>
        /// Der eingegebene Buchstabe.
        /// </summary>
        public char Character { get; set; }
    }

    /// <summary>
    /// Event Argumgents für Selektionsänderungen
    /// </summary>
    public class SelectionEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Das bisher selektiertes Item
        /// </summary>
        public T OldItem { get; set; }

        /// <summary>
        /// Das neu selektierte Item
        /// </summary>
        public T NewItem { get; set; }
    }

    /// <summary>
    /// Parameter für Collection-Events
    /// </summary>
    public class CollectionEventArgs : EventArgs
    {
        /// <summary>
        /// Das betroffene Control
        /// </summary>
        public Control Control { get; set; }

        /// <summary>
        /// Der betroffene Index
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// Parameter für Screen-Navigationsevents
    /// </summary>
    public class NavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Soll die Navigation an dieser Stelle abgebrochen werden?
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Übergabe-Parameter aus dem anderen Screen.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Gibt den zweiten an der Navigation beteiligten Screen hat.
        /// </summary>
        public Screen Screen { get; set; }

        /// <summary>
        /// Gibt an ob es sich dabei um eine Back-Navigation handelt oder nicht.
        /// </summary>
        public bool IsBackNavigation { get; set; }
    }

    /// <summary>
    /// Standard Event-Delegate ohne größeren Parameter.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void EventDelegate(Control sender, EventArgs args);

    /// <summary>
    /// Event-Delegat für Drag-Events
    /// </summary>
    /// <param name="args"></param>
    public delegate void DragEventDelegate(DragEventArgs args);

    /// <summary>
    /// Event Delegat für Maus-Events.
    /// </summary>
    /// <param name="sender">Aufrufendes Control</param>
    /// <param name="args">Eventargumente</param>
    public delegate void MouseEventDelegate(Control sender, MouseEventArgs args);

    /// <summary>
    /// Event Delegat für Mouse-Events
    /// </summary>
    /// <param name="args"></param>
    public delegate void MouseEventBaseDelegate(MouseEventArgs args);

    /// <summary>
    /// Event-Delegat für Maus-Scroll-Events.
    /// </summary>
    /// <param name="sender">Aufrufendes Control</param>
    /// <param name="args">Eventargumente</param>
    public delegate void MouseScrollEventDelegate(Control sender, MouseScrollEventArgs args);

    public delegate void MouseScrollEventBaseDelegate(MouseScrollEventArgs args);

    public delegate void TouchEventDelegate(Control control, TouchEventArgs args);

    public delegate void TouchEventBaseDelegate(TouchEventArgs args);

    /// <summary>
    /// Event Delegat für Keyboard-Events.
    /// </summary>
    /// <param name="sender">Aufrufendes Control</param>
    /// <param name="args">Eventargumente</param>
    public delegate void KeyEventDelegate(Control sender, KeyEventArgs args);

    /// <summary>
    /// Event Delegat für KeyDown im ScreenManager
    /// </summary>
    /// <param name="args">Eventargumente</param>
    public delegate void KeyEventBaseDelegate(KeyEventArgs args);

    /// <summary>
    /// Event Delegat für Texteingabe-Events
    /// </summary>
    /// <param name="sender">Aufrufendes Control</param>
    /// <param name="args">Eventargumente</param>
    public delegate void KeyTextEventDelegate(Control sender, KeyTextEventArgs args);

    /// <summary>
    /// Delegat für Events rum um Selektionsänderungen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">Eventargumente</param>
    public delegate void SelectionDelegate<T>(Control sender, SelectionEventArgs<T> args);

    public delegate void CollectionDelegate(Control sender, CollectionEventArgs args);

    /// <summary>
    /// Event Delegat für PropertyChanged-Events
    /// </summary>
    /// <typeparam name="T">Typ der Property</typeparam>
    /// <param name="sender">Aufrufendes Control</param>
    /// <param name="args">Eventargumente</param>
    public delegate void PropertyChangedDelegate<T>(Control sender, PropertyEventArgs<T> args);
}
