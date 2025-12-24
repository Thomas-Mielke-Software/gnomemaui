// using System.Runtime.InteropServices;
// using GObject;
// using GObject.Internal;
// using Gtk;
// using Gtk.Internal;

// namespace Microsoft.Maui;

// public class TestPanel : Gtk.Widget
// {
// 	static GObject.Type? _gtype;

// 	// Static references to delegates to avoid GC
// 	static GObject.Internal.ClassInitFunc? _classInitDelegate;
// 	static WidgetClassData.MeasureCallback? _measureCallback;

// 	static TestPanel()
// 	{
// 		_classInitDelegate = DoClassInit;
// 		_measureCallback = OnMeasureNative;

// 		_gtype = RegisterType();
// 	}

// 	static GObject.Type RegisterType()
// 	{
// 		var parentType = Gtk.Widget.GetGType();
// 		var parentTypeInfo = TypeQueryOwnedHandle.Create();
// 		GObject.Internal.Functions.TypeQuery(parentType, parentTypeInfo);

// 		var handle = TypeInfoOwnedHandle.Create();
// 		handle.SetClassSize((ushort)parentTypeInfo.GetClassSize());
// 		handle.SetInstanceSize((ushort)parentTypeInfo.GetInstanceSize());

// 		// Install ClassInit callback (keep delegate alive in static field)
// 		handle.SetClassInit(_classInitDelegate!);

// 		var typeName = "TestPanel";
// 		var typeId = GObject.Internal.Functions.TypeRegisterStatic(
// 			parentType,
// 			GLib.Internal.NonNullableUtf8StringOwnedHandle.Create(typeName),
// 			handle,
// 			0
// 		);

// 		var newType = new GObject.Type(typeId);
// 		DynamicInstanceFactory.Register(newType, (ptr, owns) => new TestPanel(new Gtk.Internal.WidgetHandle(ptr, owns)));

// 		return newType;
// 	}

// 	// ClassInit: set vfuncs
// 	static void DoClassInit(IntPtr gClass, IntPtr classData)
// 	{
// 		var data = Marshal.PtrToStructure<WidgetClassData>(gClass);
// 		data.Measure = _measureCallback!;
// 		Marshal.StructureToPtr(data, gClass, false);
// 	}

// 	// Native callback invoked by GTK when measuring
// 	static void OnMeasureNative(IntPtr widgetPtr, Gtk.Orientation orientation, int forSize,
// 		out int minimum, out int natural, out int minimumBaseline, out int naturalBaseline)
// 	{
// 		var widget = (TestPanel)InstanceWrapper.WrapHandle<TestPanel>(widgetPtr, false);
// 		widget.OnMeasure(orientation, forSize, out minimum, out natural, out minimumBaseline, out naturalBaseline);
// 	}

// 	// Internal constructor used by the instance factory
// 	internal TestPanel(Gtk.Internal.WidgetHandle handle) : base(handle) { }

// 	// Public constructor for C# callers
// 	public TestPanel(params GObject.ConstructArgument[] constructArguments) : this(Gtk.Internal.WidgetHandle.For<TestPanel>(constructArguments)) { }

// 	// Virtual method that derived classes can override
// 	protected virtual void OnMeasure(Gtk.Orientation orientation, int forSize,
// 		out int minimum, out int natural, out int minimumBaseline, out int naturalBaseline)
// 	{
// 		// Default measure: no minimum, some natural size
// 		minimum = 0;
// 		natural = 100;
// 		minimumBaseline = -1;
// 		naturalBaseline = -1;
// 	}
// }
