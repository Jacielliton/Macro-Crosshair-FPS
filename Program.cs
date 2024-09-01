using System;
using System.Drawing;
using System.Windows.Forms;

namespace CrosshairSelectorApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private ComboBox crosshairTypeComboBox;
        private ComboBox colorComboBox;
        private Button showCrosshairButton;
        private Label instructionLabel;
        private Label crosshairTypeLabel;
        private Label colorLabel;

        public MainForm()
        {
            Text = "Crosshair Selector";
            Width = 350;
            Height = 250;

            // Usar FlowLayoutPanel para organizar os controles
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            // Label para o tipo de mira
            crosshairTypeLabel = new Label
            {
                Text = "Selecione o tipo de mira:",
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 5)
            };

            // Criar ComboBox para selecionar tipo de mira
            crosshairTypeComboBox = new ComboBox
            {
                Items = { "Cross", "Circle", "Dot" },
                SelectedIndex = 0,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };

            // Label para a cor da mira
            colorLabel = new Label
            {
                Text = "Selecione a cor da mira:",
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 5)
            };

            // Criar ComboBox para selecionar a cor da mira
            colorComboBox = new ComboBox
            {
                Items = { "Red", "Green", "Blue", "Yellow", "White" },
                SelectedIndex = 0,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };

            // Criar botão para exibir a mira
            showCrosshairButton = new Button
            {
                Text = "Show Crosshair",
                Dock = DockStyle.Bottom
            };
            showCrosshairButton.Click += ShowCrosshairButton_Click;

            // Adicionar Label com a instrução para fechar
            instructionLabel = new Label
            {
                Text = "Pressione Ctrl + Alt + K para fechar a sobreposição",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 10, 0, 0)
            };

            // Adicionar os controles ao painel
            panel.Controls.Add(crosshairTypeLabel);
            panel.Controls.Add(crosshairTypeComboBox);
            panel.Controls.Add(colorLabel);
            panel.Controls.Add(colorComboBox);

            // Adicionar controles ao formulário
            Controls.Add(panel);
            Controls.Add(showCrosshairButton);
            Controls.Add(instructionLabel);
        }

        private void ShowCrosshairButton_Click(object? sender, EventArgs e)
        {
            string selectedType = crosshairTypeComboBox.SelectedItem?.ToString() ?? "Cross";
            string selectedColorName = colorComboBox.SelectedItem?.ToString() ?? "Red";
            Color selectedColor = Color.FromName(selectedColorName);

            Form overlayForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized,
                BackColor = Color.Magenta,
                TransparencyKey = Color.Magenta,
                TopMost = true,
                ShowInTaskbar = false
            };

            // Adicionar suporte para fechar com Ctrl + Alt + K
            overlayForm.KeyPreview = true;
            overlayForm.KeyDown += (snd, evt) =>
            {
                if (evt.Control && evt.Alt && evt.KeyCode == Keys.K)
                {
                    overlayForm.Close();
                }
            };

            overlayForm.Paint += (snd, evt) =>
            {
                int centerX = overlayForm.ClientSize.Width / 2;
                int centerY = overlayForm.ClientSize.Height / 2;

                Pen pen = new Pen(selectedColor, 2);

                if (selectedType == "Cross")
                {
                    // Linha horizontal
                    evt.Graphics.DrawLine(pen, centerX - 20, centerY, centerX + 20, centerY);
                    // Linha vertical
                    evt.Graphics.DrawLine(pen, centerX, centerY - 20, centerX, centerY + 20);
                }
                else if (selectedType == "Circle")
                {
                    // Circulo
                    evt.Graphics.DrawEllipse(pen, centerX - 20, centerY - 20, 40, 40);
                }
                else if (selectedType == "Dot")
                {
                    // Ponto
                    evt.Graphics.FillEllipse(new SolidBrush(selectedColor), centerX - 5, centerY - 5, 10, 10);
                }
            };

            overlayForm.ShowDialog();
        }
    }
}
