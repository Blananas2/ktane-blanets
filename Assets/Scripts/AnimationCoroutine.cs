﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ut {
    public static IEnumerator Animation(float duration, Action<float> action)
    {
        float delta = 0;
        while (delta < 1)
        {
            yield return null;
            delta += Time.deltaTime / duration;
            action(delta);
        }
    }
    public static IEnumerator Press(KMSelectable btn, float delay)
    {
        btn.OnInteract();
        yield return new WaitForSeconds(delay);
    }
}
