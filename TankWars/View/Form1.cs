// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;

namespace View
{
    public partial class Form1 : Form
    {
        GameController theController;
        World theWorld;

        private const int viewSize = 900;
        private const int menuSize = 40;

        DrawingPanel drawingPanel;
        Button startButton;
        Label nameLabel;
        TextBox nameText;
        Label addrLabel;
        TextBox addrText;

        public Form1()
        {
            InitializeComponent();
            theController = new GameController(viewSize);
            theWorld = theController.getWorld();
            theController.UpdateArrived += OnFrame;
            theController.ErrorEvent += NetworkError;

            // Set the window size
            ClientSize = new Size(viewSize, viewSize + menuSize);

            // Place and add the button
            startButton = new Button();
            startButton.Location = new Point(300, 5);
            startButton.Size = new Size(70, 20);
            startButton.Text = "Start";
            startButton.Click += StartClick;
            this.Controls.Add(startButton);

            // Place and add the name label
            nameLabel = new Label();
            nameLabel.Text = "Name:";
            nameLabel.Location = new Point(5, 10);
            nameLabel.Size = new Size(40, 15);
            this.Controls.Add(nameLabel);

            // Place and add the name textbox
            nameText = new TextBox();
            nameText.Text = "player";
            nameText.Location = new Point(50, 5);
            nameText.Size = new Size(70, 15);
            this.Controls.Add(nameText);

            // Place and add the address label
            addrLabel = new Label();
            addrLabel.Text = "Server:";
            addrLabel.Location = new Point(130, 10);
            addrLabel.Size = new Size(40, 15);
            this.Controls.Add(addrLabel);

            // Place and add the address textbox
            addrText = new TextBox();
            addrText.Text = "localhost";
            addrText.Location = new Point(175, 5);
            addrText.Size = new Size(70, 15);
            this.Controls.Add(addrText);

            // Place and add the drawing panel
            drawingPanel = new DrawingPanel(theWorld);
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            drawingPanel.BackColor = Color.LightGreen;
            this.Controls.Add(drawingPanel);

            // Set up key and mouse handlers
            this.KeyDown += HandleKeyDown;
            this.KeyUp += HandleKeyUp;
            drawingPanel.MouseDown += HandleMouseDown;
            drawingPanel.MouseUp += HandleMouseUp;
            drawingPanel.MouseMove += HandleMouseMovement;
        }

        /// <summary>
        /// Handler for the controller's UpdateArrived event
        /// </summary>
        private void OnFrame()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            try
            {
                MethodInvoker invalidator = new MethodInvoker(() => this.Invalidate(true));
                this.Invoke(invalidator);
            }
            catch (Exception)
            {
                // Exit the program
            }

        }

        /// <summary>
        /// Handler for the controller's ErrorOccured event
        /// </summary>
        private void NetworkError()
        {
            MethodInvoker error = new MethodInvoker
            ( () =>
                {
                    startButton.Enabled = true;
                    nameText.Enabled = true;
                    addrText.Enabled = true;
                    KeyPreview = false;
                    MessageBox.Show("Networking Error");
                }
            );
            this.Invoke(error);
        }

        /// <summary>
        /// Connenct to the provided server address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartClick(object sender, EventArgs e)
        {
            // Disable the form controls
            startButton.Enabled = false;
            nameText.Enabled = false;
            addrText.Enabled = false;
            // Enable the global form to capture key presses
            KeyPreview = true;
            // connect to the server
            theController.connectToServer(nameText.Text, addrText.Text);
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
            else
            {
                theController.HandleKeyRequest(e.KeyCode);
            }

            // Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        /// <summary>
        /// Key up handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            theController.CancelKeyRequest(e.KeyCode);
        }

        /// <summary>
        /// Handle mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            theController.HandleMouseRequest(e.Button);
        }

        /// <summary>
        /// Handle mouse up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            theController.CancelMouseRequest();
        }

        /// <summary>
        /// Handle mouse movement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseMovement(object sender, MouseEventArgs e)
        {
            theController.MouseMoved(e.X, e.Y);
        }

    }
}
