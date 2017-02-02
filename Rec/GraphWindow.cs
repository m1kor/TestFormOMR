using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rec {
    public partial class GraphWindow : Form {
        private MainWindow mainWindow;

        public GraphWindow(MainWindow mainWindow) {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void GraphWindow_Load(object sender, EventArgs e) {
            mainWindow.DrawToolGraph();
        }

        private void GraphWindow_Resize(object sender, EventArgs e) {
            mainWindow.DrawToolGraph();
        }
    }
}
