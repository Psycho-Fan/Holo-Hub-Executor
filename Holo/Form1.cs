using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO; // For File and Directory
using cxapi;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Holo
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        // Variables to track the mouse position for dragging
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private string scriptsFolder = @"scripts"; // Change to your actual folder path

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Attach the mouse events to the specific control (panel1) for dragging
            this.panel1.MouseDown += new MouseEventHandler(Panel_MouseDown);
            this.panel1.MouseMove += new MouseEventHandler(Panel_MouseMove);
            this.panel1.MouseUp += new MouseEventHandler(Panel_MouseUp);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadFiles(); // Load files when the form loads
        }

        private void LoadFiles()
        {
            // Get all .txt and .lua files from the scripts folder
            var files = Directory.GetFiles(scriptsFolder, "*.*")
                                 .Where(file => file.EndsWith(".txt") || file.EndsWith(".lua"))
                                 .ToList(); // Convert to a list to check count later

            // Add the files to the ListBox only if any exist
            listBox1.Items.Clear();
            if (files.Count > 0) // Check if there are any files
            {
                foreach (var file in files)
                {
                    listBox1.Items.Add(Path.GetFileName(file)); // Add only the file name
                }
            }
            // If no files are found, listBox1 will remain empty and no error will be shown
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected file name
            string selectedFileName = listBox1.SelectedItem as string;
            if (selectedFileName != null)
            {
                // Construct the full file path
                string filePath = Path.Combine(scriptsFolder, selectedFileName);

                // Read the file content and display it in the TextBox
                try
                {
                    textBox1.Text = File.ReadAllText(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Close button click event
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes the form
        }

        // Minimize button click event
        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized; // Minimizes the form
        }

        // Handle MouseDown event on the panel1 to start dragging
        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        // Handle MouseMove event on the panel1 to drag the form
        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        // Handle MouseUp event on the panel1 to stop dragging
        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cxapi.CoreFunctions.Inject();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string script = textBox1.Text;
            cxapi.CoreFunctions.ExecuteScript(script);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Optional: Add custom painting code here if needed
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {
            // Optional: Add custom painting code here if needed
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Toggle the TopMost property
            this.TopMost = !this.TopMost;
        }
    }
}