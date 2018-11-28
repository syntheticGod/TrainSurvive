/*
 * 描述：测试科技
 * 作者：刘旭涛
 * 创建时间：2018/11/25 23:53:23
 * 版本：v0.1
 */

using UnityEngine;

public class TEST_Tech0 : Tech {
    public override int[] Dependencies { get; } = { };
    public override string Name { get; } = "T0";
    public override string Description { get; } = "BLA0";
    public override float TotalWorks { get; } = 5;
    public override void OnCompleted() {
        Debug.Log(Name);
    }
}
public class TEST_Tech1 : Tech {
    public override int[] Dependencies { get; } = { 0 };
    public override string Name { get; } = "T1";
    public override string Description { get; } = "BLA1";
    public override float TotalWorks { get; } = 5;
    public override void OnCompleted() {
        Debug.Log(Name);
    }
}
public class TEST_Tech2 : Tech {
    public override int[] Dependencies { get; } = { 1 };
    public override string Name { get; } = "T2";
    public override string Description { get; } = "BLA2";
    public override float TotalWorks { get; } = 1;
    public override void OnCompleted() {
        Debug.Log(Name);
    }
}
public class TEST_Tech3 : Tech {
    public override int[] Dependencies { get; } = { 0 };
    public override string Name { get; } = "T3";
    public override string Description { get; } = "BLA3";
    public override float TotalWorks { get; } = 1;
    public override void OnCompleted() {
        Debug.Log(Name);
    }
}
public class TEST_Tech4 : Tech {
    public override int[] Dependencies { get; } = { 2, 3 };
    public override string Name { get; } = "T4";
    public override string Description { get; } = "BLA4";
    public override float TotalWorks { get; } = 1;
    public override void OnCompleted() {
        Debug.Log(Name);
    }
}
public class TEST_Tech5 : Tech {
    public override int[] Dependencies { get; } = { 0 };
    public override string Name { get; } = "T5";
    public override string Description { get; } = "BLA5";
    public override float TotalWorks { get; } = 1;
    public override void OnCompleted() {
        Debug.Log(Name);
    }
}
public class TEST_Tech6 : Tech {
    public override int[] Dependencies { get; } = { 1, 5 };
    public override string Name { get; } = "T6";
    public override string Description { get; } = "BLA6";
    public override float TotalWorks { get; } = 1;
    public override void OnCompleted() {
        Debug.Log(Name);
    }
}
