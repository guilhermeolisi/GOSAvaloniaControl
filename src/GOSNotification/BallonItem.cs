using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSAvaloniaControls;

internal class BallonItem
{
    public BallonItem(NotificationItem item, DateTime time)
    {
        Item = item;
        Time = time;
    }

    public NotificationItem Item { get; set; }
    public DateTime Time { get; set; }

}
