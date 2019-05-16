using System.Windows;
using Infragistics.Controls.Menus;

namespace SC.Base.WPF.Views
{
	public class CustXamDataTree : XamDataTree
	{
		//private bool IsSetFromNodeChanged = false;
		private static XamDataTreeNodesCollection nodes;

		public CustXamDataTree()
		{
			ActiveNodeChanged += CustXamDataTree_ActiveNodeChanged;
			MouseLeftButtonDown += CustXamDataTree_MouseLeftButtonDown;
			nodes = Nodes;
		}

		void CustXamDataTree_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{

			Infragistics.Controls.Menus.Primitives.NodesPanel myEventArgs = e.OriginalSource as Infragistics.Controls.Menus.Primitives.NodesPanel;
			if (myEventArgs != null)
			{
				CustomUnselectNode();
			}

		}

		void CustXamDataTree_ActiveNodeChanged(object sender, ActiveNodeChangedEventArgs e)
		{
			// IsSetFromNodeChanged = true;
			// START modified by HPW
			if (e.NewActiveTreeNode != null)
			{
				SelectedItem = e.NewActiveTreeNode.Data;
			}

            //END
		}

		public object SelectedItem
		{
			get { return GetValue(SelectedItemsProperty); }
			set
			{
				SetValue(SelectedItemsProperty, value);

			}
		}

		private static void FindCorrespondingNode(XamDataTreeNodesCollection nodes, object value)
		{
			foreach (XamDataTreeNode node in nodes)
			{
				bool isSelected = node.Data.Equals(value);
				node.IsActive = isSelected;
				node.IsSelected = isSelected;

				if (node.HasChildren)
				{
					FindCorrespondingNode(node.Nodes, value);
				}
			}
		}

		public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
			"SelectedItem",
			typeof(object),
			typeof(CustXamDataTree),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
				OnPropertyChangedCallback));

		public static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FindCorrespondingNode(nodes, e.NewValue);
		}

		public void CustomUnselectNode()
		{
			//	UnselectNode(this.SelectedItem as XamDataTreeNode);
			SelectedItem = null;
		}
	}
}
