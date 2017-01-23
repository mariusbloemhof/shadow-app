using System;
using System.Collections.Generic;
using System.Text;

namespace Shadow.Model
{
    public class ErrorEventArgs : EventArgs
    {
        private readonly AccountResult _result;

        public ErrorEventArgs(AccountResult result)
        {
            _result = result;
        }

        public AccountResult Result
        {
            get { return _result; }
        }
    }
}
