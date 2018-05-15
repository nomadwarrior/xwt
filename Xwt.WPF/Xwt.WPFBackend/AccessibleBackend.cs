using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using Xwt.Accessibility;
using Xwt.Backends;

namespace Xwt.WPFBackend
{
	class AccessibleBackend : IAccessibleBackend
	{
		UIElement element;
		IAccessibleEventSink eventSink;
		ApplicationContext context;

		public bool IsAccessible { get; set; }

		public string Identifier {
			get { return AutomationProperties.GetAutomationId (element); }
			set { AutomationProperties.SetAutomationId (element, value); }
		}
		public string Label {
			get { return AutomationProperties.GetName (element); }
			set { AutomationProperties.SetName (element, value); }
		}
		public string Description {
			get { return AutomationProperties.GetHelpText (element); }
			set { AutomationProperties.SetHelpText (element, value); }
		}
		public Widget LabeledBy {
			set {
				AutomationProperties.SetLabeledBy (element, (Toolkit.GetBackend (value) as WidgetBackend)?.Widget);
			}
		}

		public string Title { get; set; }
		public string Value { get; set; }
		public Role Role { get; set; } = Role.Custom;
		public Uri Uri { get; set; }
		public Rectangle Bounds { get; set; }
		public string RoleDescription { get; set; }

		public void DisableEvent (object eventId)
		{
		}

		public void EnableEvent (object eventId)
		{
		}

		public void Initialize (IWidgetBackend parentWidget, IAccessibleEventSink eventSink)
		{
			Initialize (parentWidget.NativeWidget, eventSink);
		}

		public void Initialize (object parentWidget, IAccessibleEventSink eventSink)
		{
			this.element = parentWidget as UIElement;
			if (element == null)
				throw new ArgumentException ("Widget is not a UIElement");
			this.eventSink = eventSink;
		}

		public void InitializeBackend (object frontend, ApplicationContext context)
		{
			this.context = context;
		}

		internal void PerformInvoke ()
		{
			context.InvokeUserCode (() => eventSink.OnPress ());
		}

		// The following child methods are only supported for Canvas based widgets
		public void AddChild (object nativeChild)
		{
			var peer = nativeChild as AutomationPeer;
			var canvas = element as CustomCanvas;
			if (peer != null && canvas != null)
				canvas.AutomationPeer?.AddChild (peer);
		}

		public void RemoveAllChildren ()
		{
			var canvas = element as CustomCanvas;
			if (canvas != null)
				canvas.AutomationPeer?.RemoveAllChildren ();
		}

		public void RemoveChild (object nativeChild)
		{
			var peer = nativeChild as AutomationPeer;
			var canvas = element as CustomCanvas;
			if (peer != null && canvas != null)
				canvas.AutomationPeer?.RemoveChild (peer);
		}

		public static AutomationControlType RoleToControlType (Role role)
		{
			switch (role) {
			case Role.Button:
			case Role.MenuButton:
			case Role.ToggleButton:
				return AutomationControlType.Button;
			case Role.CheckBox:
				return AutomationControlType.CheckBox;
			case Role.RadioButton:
			case Role.RadioGroup:
				return AutomationControlType.RadioButton;
			case Role.ComboBox:
				return AutomationControlType.ComboBox;
			case Role.List:
				return AutomationControlType.List;
			case Role.Popup:
			case Role.ToolTip:
				return AutomationControlType.ToolTip;
			case Role.ToolBar:
				return AutomationControlType.ToolBar;
			case Role.Label:
				return AutomationControlType.Text;
			case Role.Link:
				return AutomationControlType.Hyperlink;
			case Role.Image:
				return AutomationControlType.Image;
			case Role.Cell:
				return AutomationControlType.DataItem;
			case Role.Table:
				return AutomationControlType.Table;
			case Role.Paned:
				return AutomationControlType.Pane;
			default:
				return AutomationControlType.Custom;
			}
		}
	}
}
