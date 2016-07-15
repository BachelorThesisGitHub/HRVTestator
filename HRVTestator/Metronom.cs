using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;

namespace HRVTestator
{
    class Metronom
    {
        public int counter = 0;
        public Timer tmr;
    }
}