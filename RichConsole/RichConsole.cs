﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RichConsole.Argument;

namespace ConsoleApp
{
    public partial class ConsoleManager : Form, RichConsole.IConsoleApp
    {
        public ConsoleManager()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void CreateControlQueue<T>(int count) where T : Control, new()
        {
            for (int i = 0; i < count; i++)
            {
                T control = new T();
                control.Name = "ManagedControl";
                control.Enabled = false;
                control.Visible = false;
                control.Font = new Font("돋움체", 12f, FontStyle.Bold);

                //CleanControl(control);

                Controls.Add(control);
                ControlQueueCount++;
            }
        }
        public T GetControl<T>() where T : Control
        {

            foreach (Control control in Controls)
            {
                if (control.Name == "ManagedControl")
                {
                    if ((!control.Enabled) && (!control.Visible))
                    {
                        Invoke((MethodInvoker)delegate ()
                        {
                            control.Enabled = true;
                            control.Visible = true;
                            control.Tag = null;
                        });
                        return control as T;
                    }
                }
            }
            throw new Exception("out of control queue");
        }
        public void DisposeControl(Control control)
        {
            if (control.Name == "ManagedControl")
            {
                Invoke((MethodInvoker)delegate ()
                {
                    control.Enabled = false;
                    control.Visible = false;
                    /*
                    (control.Tag as WriteAble.Complexed).font.Dispose();
                    control.Font.Dispose();
                    */
                    CleanControl(control);
                });
            }
            else
            {
                //throw new Exception(control.Name + " is not ManagedControl");
            }
        }
        private void CleanControl(Control ctrl)
        {
            ctrl.MouseEnter -= MouseEnterEvent;
            ctrl.MouseLeave -= MouseLeaveEvent;
            ctrl.MouseClick -= ButtonClickEvent;
        }

        public int ControlQueueCount = 0;

        private void CreateText(TextObject textobj)
        {
            Invoke((MethodInvoker)delegate ()
            {
                TextBox ctrl = GetControl<TextBox>();
                WriteAble.Text text = textobj.data as WriteAble.Text;

                ctrl.Location = new Point(textobj.location.X, textobj.location.Y);
                ctrl.Height = textobj.location.Height;
                ctrl.Width = textobj.location.Width;

                //오류나는곳
                /*
                if (!ctrl.Font.Equals(text.font) && ctrl.Font != text.font)
                {
                    ctrl.Font = text.font;
                }
                */

                ctrl.Tag = text;

                ctrl.Margin = new Padding(0, 0, 0, 0);
                ctrl.ForeColor = text.textColor.basecolor;
                ctrl.BackColor = text.textColor.backcolor;

                ctrl.MouseEnter += MouseEnterEvent;
                ctrl.MouseLeave += MouseLeaveEvent;


                ctrl.ReadOnly = true;
                ctrl.BorderStyle = BorderStyle.None;
                ctrl.Text = text.str;
            });
        }
        private void CreateButton(TextObject textobj)
        {
            Invoke((MethodInvoker)delegate ()
            {
                TextBox ctrl = GetControl<TextBox>();
                WriteAble.Button btn = textobj.data as WriteAble.Button;

                ctrl.Location = new Point(textobj.location.X, textobj.location.Y);
                ctrl.Height = textobj.location.Height;
                ctrl.Width = textobj.location.Width;

                //오류나는곳
                /*
                if (!ctrl.Font.Equals(btn.font) && ctrl.Font != btn.font)
                {
                    ctrl.Font = btn.font;
                }
                */


                ctrl.Tag = btn;

                ctrl.Margin = new Padding(0, 0, 0, 0);
                ctrl.ForeColor = btn.textColor.basecolor;
                ctrl.BackColor = btn.textColor.backcolor;

                ctrl.MouseEnter += MouseEnterEvent;
                ctrl.MouseLeave += MouseLeaveEvent;
                ctrl.MouseClick += ButtonClickEvent;

                ctrl.ReadOnly = true;
                ctrl.BorderStyle = BorderStyle.None;
                ctrl.Text = btn.str;
            });
        }









        private void CreateInputText(TextObject textobj)
        {
            Invoke((MethodInvoker)delegate ()
            {
                TextBox ctrl = GetControl<TextBox>();
                WriteAble.InputText text = textobj.data as WriteAble.InputText;

                ctrl.Location = new Point(textobj.location.X, textobj.location.Y);
                ctrl.Height = textobj.location.Height;
                ctrl.Width = textobj.location.Width;

                //오류나는곳
                /*
                if (!ctrl.Font.Equals(text.font) && ctrl.Font != text.font)
                {
                    ctrl.Font = text.font;
                }
                */

                ctrl.Tag = text;

                ctrl.Margin = new Padding(0, 0, 0, 0);
                ctrl.ForeColor = text.textColor.basecolor;
                ctrl.BackColor = text.textColor.backcolor;

                ctrl.MouseEnter += MouseEnterEvent;
                ctrl.MouseLeave += MouseLeaveEvent;


                ctrl.ReadOnly = false;
                ctrl.BorderStyle = BorderStyle.None;
                ctrl.Text = text.str;
            });
        }
        private void MouseEnterEvent(object sender, EventArgs e)
        {
            TextColor color = ((WriteAble.Complexed)((TextBox)sender).Tag).textColor;
            ((TextBox)sender).ForeColor = color.hoverbasecolor;
            ((TextBox)sender).BackColor = color.hoverbackcolor;
        }
        private void MouseLeaveEvent(object sender, EventArgs e)
        {
            TextColor color = ((WriteAble.Complexed)((TextBox)sender).Tag).textColor;
            ((TextBox)sender).ForeColor = color.basecolor;
            ((TextBox)sender).BackColor = color.backcolor;
        }
        private void ButtonClickEvent(object sender, EventArgs e)
        {
            buttonclicked = true;
            buttonvalue = (WriteAble.Button)((TextBox)sender).Tag;
        }



        private static bool linesent = false;
        private static string linevalue;

        private static bool buttonclicked = false;
        private static WriteAble.Button buttonvalue;

        public bool Initialized { get; set; } = false;

        private void ConsoleInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    linevalue = ConsoleInput.Text;
                    linesent = true;
                    ConsoleInput.Text = "";
                    break;
            }
        }

        private static void MouseButtonClick(object sender, MouseEventArgs e)
        {

        }

        private void RichConsole_Shown(object sender, EventArgs e)
        {
            CreateControlQueue<TextBox>(100);
            Initialized = true;
        }

        private void ConsoleInput_TextChanged(object sender, EventArgs e)
        {

        }


        public void Write(TextObject obj)
        {
            switch (obj.data)
            {
                case WriteAble.Text _:
                    CreateText(obj);
                    break;
                case WriteAble.Button _:
                    CreateButton(obj);
                    break;
                case WriteAble.InputText _:
                    CreateInputText(obj);
                    break;
            }
        }

        public async Task<string> ReadLine()
        {
            while (!linesent) await Task.Delay(25);
            linesent = false;
            return linevalue;
        }

        public async Task<WriteAble.Button> ReadButton()
        {
            while (!buttonclicked) await Task.Delay(25);
            buttonclicked = false;
            return buttonvalue;
        }

        public void Clear(Predicate<WriteAble> predicate)
        {
            List<Control> removelist = new List<Control>();
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Name == "ManagedControl" && ctrl.Enabled == true && ctrl.Visible == true)
                {
                    if (predicate((WriteAble)ctrl.Tag))
                    {
                        removelist.Add(ctrl);
                    }
                }
            }
            foreach (Control ctrl in removelist)
            {
                DisposeControl(ctrl);



                //Items.Remove(ctrl);
                /*
                Invoke((MethodInvoker)delegate ()
                {
                    ctrl.Font = null;
                    Controls.Remove(ctrl);
                    ctrl.Dispose();
                });
                */
            }
        }

        public string GetInputText(string name)
        {
            foreach (Control item in Controls)
            {
                if (item.Tag is WriteAble.InputText)
                {
                    return ((TextBox)item).Text;
                }
            }
            throw new Exception("nofindname");
        }
    }
}
