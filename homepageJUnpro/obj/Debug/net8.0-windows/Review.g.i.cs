// Updated by XamlIntelliSenseFileGenerator 11/17/2024 8:35:20 PM
#pragma checksum "..\..\..\Review.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "484524993CC6DED0E9793D7F4D67ABBBF342BD9A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using homepageJUnpro;


namespace homepageJUnpro
{


    /// <summary>
    /// Review
    /// </summary>
    public partial class Review : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {

#line default
#line hidden


#line 27 "..\..\..\Review.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border pictureBox1;

#line default
#line hidden


#line 29 "..\..\..\Review.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imageControl;

#line default
#line hidden


#line 31 "..\..\..\Review.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddGambarButton;

#line default
#line hidden


#line 35 "..\..\..\Review.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ReviewButton;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.10.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/homepageJUnpro;V1.0.0.0;component/review.xaml", System.UriKind.Relative);

#line 1 "..\..\..\Review.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.10.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.NameTB = ((System.Windows.Controls.TextBox)(target));
                    return;
                case 2:
                    this.stockTB = ((System.Windows.Controls.TextBox)(target));
                    return;
                case 3:
                    this.pictureBox1 = ((System.Windows.Controls.Border)(target));
                    return;
                case 4:
                    this.imageControl = ((System.Windows.Controls.Image)(target));
                    return;
                case 5:
                    this.AddGambarButton = ((System.Windows.Controls.Button)(target));

#line 31 "..\..\..\Review.xaml"
                    this.AddGambarButton.Click += new System.Windows.RoutedEventHandler(this.AddGambarButton_Click);

#line default
#line hidden
                    return;
                case 6:
                    this.ReviewButton = ((System.Windows.Controls.Button)(target));

#line 35 "..\..\..\Review.xaml"
                    this.ReviewButton.Click += new System.Windows.RoutedEventHandler(this.ReviewButton_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }

        internal System.Windows.Controls.TextBox descTB;
        internal System.Windows.Controls.TextBox scoreTB;
    }
}

