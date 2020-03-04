using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.SqlServer.Dts.Runtime;

using Microsoft.SqlServer.Graph;
using Microsoft.SqlServer.Graph.Model;
using Microsoft.SqlServer.Graph.LayoutEngine;
using Microsoft.SqlServer.Graph.Extended;
using Microsoft.SqlServer.Graph.UI;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

using Microsoft.SqlServer.IntegrationServices.Designer;
using Microsoft.SqlServer.IntegrationServices.Designer.Model.Serialization;
using Microsoft.SqlServer.IntegrationServices.Designer.View;
using Microsoft.SqlServer.Graph.LayoutEngine.Sugiyama;
using Microsoft.SqlServer.IntegrationServices.Designer.Model;
using System.Reflection;

namespace CreatePackageDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LoadPackage();
        }

        private void LoadPackage()
        {
            string contents = String.Empty;

            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();

            using (StreamReader r = new StreamReader("DemoPackageWithoutDesignTimeProperties.dtsx"))
            {
                contents = r.ReadToEnd();
            }

            package.LoadFromXML(contents, null);

            ControlFlowGraphModelElement controlFlowGraphModelElement = new ControlFlowGraphModelElement();
            controlFlowGraphModelElement.Initialize(package as IDTSSequence);

            ModelElement m = controlFlowGraphModelElement.GetModelElement("{FFE5461A-385C-4A35-984A-91B6A8E43119}");

            GraphModelElement graphModelElement = new GraphModelElement();
            graphModelElement.Container = controlFlowGraphModelElement.Container;

            GraphLayout graphLayout = GraphLayout.GetLayout(graphModelElement);

            GraphControl graphControl = new GraphControl();
            graphControl.Width = 1000;
            graphControl.Height = 1000;
            graphControl.DataContext = m;

            GraphPanelEx p1 = new GraphPanelEx();
            p1.Width = 1000;
            p1.Height = 1000;
            p1.GraphModel = graphModelElement;
            p1.UpdateEx(true);

            GraphControlEx graphControlEx = new GraphControlEx();
            graphControlEx.DataContext = controlFlowGraphModelElement.Container;

            graphControlEx.AssignLayout(controlFlowGraphModelElement.Container, new Point(100, 100));
            graphControlEx.UpdateLayout();
          
            GraphModelElementEx graphModelElementEx = new GraphModelElementEx();
            graphModelElementEx.GraphControl = graphControl;
            graphModelElementEx.Container = controlFlowGraphModelElement.Container;
            graphModelElementEx.RefreshGraph(controlFlowGraphModelElement.Container);

            GraphPanelEx graphPanelEx = new GraphPanelEx();
            graphPanelEx.Width = 1000;
            graphPanelEx.Height = 1000;
            graphPanelEx.DataContext = graphModelElement;
            graphPanelEx.GraphModel = graphModelElement;
            graphPanelEx.LayoutEngine = new Microsoft.SqlServer.Graph.LayoutEngine.Sugiyama.MsaglLayoutGraph();
            
            graphPanelEx.UpdateEx();

            //MethodInfo dynMethod = graphPanelEx.GetType().GetMethod("CalculateLayout", BindingFlags.NonPublic | BindingFlags.Instance);
            //dynMethod.Invoke(this, new object[] { false });

            graphLayout.ApplyLayout(graphModelElementEx);

            ContainerModelElementEx x = controlFlowGraphModelElement.Container as ContainerModelElementEx;
            var children = ContainerModelElementEx.GetAllChildren(controlFlowGraphModelElement.Container);
            var bounds = ContainerModelElementEx.GetBounds(x.Children);

            graphModelElementEx.RefreshGraph(graphModelElementEx.Container);
            
            LayoutGraph layoutGraph = new LayoutGraph();
            graphLayout.AppendLayout(controlFlowGraphModelElement.Container.Children);
            graphLayout.ApplyLayout(controlFlowGraphModelElement);

            // Should contain the serialized GraphLayout XML Data with correct coordinates...
            var graphLayoutXml = SerializerHelper.Save(graphModelElement);

        }
    }
}
