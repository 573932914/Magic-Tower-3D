using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnrichedButton : Button
{
    public UnityEvent DisabledEvent = new UnityEvent();
    public UnityEvent HighlightedEvent = new UnityEvent();
    public UnityEvent NormalEvent = new UnityEvent();
    public int State
    {
        get
        {
            int x = (int)currentSelectionState;
            return x;
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        switch (state)
        {
            case SelectionState.Disabled:
                DisabledEvent.Invoke();
                break;
            case SelectionState.Highlighted:
                HighlightedEvent.Invoke();
                break;
            case SelectionState.Normal:
                NormalEvent.Invoke();
                break;
            default:
                break;
        }
    }
}
