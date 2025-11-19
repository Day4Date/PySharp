using System;
using System.Windows.Forms;
using Python.Runtime;

namespace PyCross.API.GUI.Controls
{
    public class ButtonWrapper : GuiControlWrapper
    {
        private PyObject _callback;

        public ButtonWrapper(Button btn, Form1 form, PyObject callback)
            : base(btn, form)
        {
            _callback = callback;

            btn.Click += (sender, args) =>
            {
                using (Py.GIL())
                {
                    try
                    {
                        _callback.Invoke();
                    }
                    catch (PythonException ex)
                    {
                        form.AppendLog(ex.ToString());
                    }
                }
            };
        }
    }
}
