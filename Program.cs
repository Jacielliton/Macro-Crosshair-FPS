using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinFormsApp
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
        private ComboBox weaponComboBox;
        private ComboBox crosshairTypeComboBox;
        private ComboBox colorComboBox;
        private Button showCrosshairButton;
        private Label instructionLabel;
        private Label crosshairTypeLabel;
        private Label colorLabel;
        private Label weaponLabel;
        private System.Windows.Forms.Timer fireTimer;
        private bool isMacroEnabled;
        private int recoilIndex;

        // Armazenar padrões de recuo para diferentes armas
        private Dictionary<string, int[]> weaponRecoils;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        private const int VK_LBUTTON = 0x01; // Código do botão esquerdo do mouse
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        public MainForm()
        {
            Text = "Crosshair & Macro Selector- Youtube.com/@tecnopriv.top1";
            Width = 350;
            Height = 400;  // Aumentar a altura do formulário

            // Ajustar padrões de recuo conforme solicitado
            weaponRecoils = new Dictionary<string, int[]>
            {
                { "SMG (Recuo Leve)", new int[] { 1, 1, 1, 2, 2, 2, 2, 3, 3, 3 } }, // Recuo reduzido para SMG
                { "Rifle 556 (Recuo Médio)", new int[] { 1, 1, 2, 2, 3, 3, 3, 4, 4, 4 } }, // Recuo reduzido para Rifle 556
                { "Rifle 762 (Recuo Alto)", new int[] { 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 } } // Recuo alto para Rifle 762
            };

            // Usar FlowLayoutPanel para organizar os controles
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            // Label para a arma
            weaponLabel = new Label
            {
                Text = "Selecione a arma:",
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 5)
            };

            // Criar ComboBox para selecionar a arma
            weaponComboBox = new ComboBox
            {
                Items = { "SMG (Recuo Leve)", "Rifle 556 (Recuo Médio)", "Rifle 762 (Recuo Alto)" },
                SelectedIndex = 0,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };

            // Label para o tipo de mira
            crosshairTypeLabel = new Label
            {
                Text = "Selecione o tipo de mira:",
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 5)
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
                Text = "Pressione Ctrl + Alt + I para iniciar a macro\nPressione Ctrl + Alt + J para parar a macro\nPressione Ctrl + Alt + K para fechar a sobreposição\nBy Youtube.com/@tecnopriv.top1",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 10, 0, 0),
                AutoSize = true
            };

            // Adicionar os controles ao painel
            panel.Controls.Add(weaponLabel);
            panel.Controls.Add(weaponComboBox);
            panel.Controls.Add(crosshairTypeLabel);
            panel.Controls.Add(crosshairTypeComboBox);
            panel.Controls.Add(colorLabel);
            panel.Controls.Add(colorComboBox);

            // Adicionar controles ao formulário
            Controls.Add(panel);
            Controls.Add(instructionLabel);
            Controls.Add(showCrosshairButton);
            

            // Configurar o Timer para disparos automáticos
            fireTimer = new System.Windows.Forms.Timer();
            fireTimer.Interval = 10; // Intervalo de verificação do estado do botão de tiro (menor para maior precisão)
            fireTimer.Tick += FireTimer_Tick;

            // Atalhos de teclado
            KeyPreview = true;
            KeyDown += MainForm_KeyDown;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Iniciar o macro com Ctrl + Alt + I
            if (e.Control && e.Alt && e.KeyCode == Keys.I)
            {
                if (!isMacroEnabled)
                {
                    isMacroEnabled = true;
                    recoilIndex = 0;
                    fireTimer.Start();
                }
            }

            // Parar o macro com Ctrl + Alt + J
            if (e.Control && e.Alt && e.KeyCode == Keys.J)
            {
                isMacroEnabled = false;
                fireTimer.Stop();
                SimulateMouseRelease(); // Certifique-se de liberar o botão de tiro
            }
        }

        private void FireTimer_Tick(object sender, EventArgs e)
        {
            // Verificar se o botão esquerdo do mouse está pressionado
            if (GetAsyncKeyState(VK_LBUTTON) < 0)
            {
                SimulateMouseHold();
                ApplyRecoil();
            }
            else
            {
                SimulateMouseRelease();
            }
        }

        private void SimulateMouseHold()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        private void SimulateMouseRelease()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private void ApplyRecoil()
        {
            string selectedWeapon = weaponComboBox.SelectedItem?.ToString();
            if (selectedWeapon != null && weaponRecoils.ContainsKey(selectedWeapon))
            {
                int[] recoilPattern = weaponRecoils[selectedWeapon];
                if (recoilIndex < recoilPattern.Length)
                {
                    mouse_event(MOUSEEVENTF_MOVE, 0, (uint)recoilPattern[recoilIndex], 0, 0); // Movimentar o mouse para baixo
                    recoilIndex++;
                }
                else
                {
                    recoilIndex = 0; // Reiniciar o padrão de recuo
                }
            }
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

            // Adicionar suporte para iniciar e parar o macro com atalhos
            overlayForm.KeyPreview = true;
            overlayForm.KeyDown += (snd, evt) =>
            {
                // Iniciar o macro com Ctrl + Alt + I na sobreposição
                if (evt.Control && evt.Alt && evt.KeyCode == Keys.I)
                {
                    if (!isMacroEnabled)
                    {
                        isMacroEnabled = true;
                        recoilIndex = 0;
                        fireTimer.Start();
                    }
                }

                // Parar o macro com Ctrl + Alt + J na sobreposição
                if (evt.Control && evt.Alt && evt.KeyCode == Keys.J)
                {
                    isMacroEnabled = false;
                    fireTimer.Stop();
                    SimulateMouseRelease(); // Certifique-se de liberar o botão de tiro
                }

                // Fechar a sobreposição com Ctrl + Alt + K
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
