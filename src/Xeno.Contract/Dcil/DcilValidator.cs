namespace Xeno.Contract.Dcil;

/// <summary>Structural validation of a DCIL method: operands present and in range. Does not check stack typing.</summary>
public static class DcilValidator
{
    public static IReadOnlyList<string> Validate(DcilMethod method)
    {
        var errors = new List<string>();
        var n = method.Body.Count;

        for (var i = 0; i < method.Locals.Count; i++)
            if (!DcilType.IsValid(method.Locals[i]))
                errors.Add($"local {i}: invalid type '{method.Locals[i]}'");

        for (var i = 0; i < n; i++)
        {
            var ins = method.Body[i];
            void Err(string msg) => errors.Add($"[{i}] {ins.Op.Name()}: {msg}");
            void NeedTarget(int? t) { if (t is null || t < 0 || t >= n) Err($"branch target {t} out of range 0..{n - 1}"); }

            switch (ins.Op)
            {
                case DcilOp.PushConst:
                    if (ins.Const is null) Err("missing const");
                    else if (ins.Const.Kind == DcilConstKind.Str && ins.Const.S is null) Err("string const without value");
                    break;
                case DcilOp.LoadArg or DcilOp.StoreArg:
                    if (ins.Index is null or < 0) Err("missing arg index");
                    break;
                case DcilOp.LoadLocal or DcilOp.StoreLocal:
                    if (ins.Index is null || ins.Index < 0 || ins.Index >= method.Locals.Count)
                        Err($"local index {ins.Index} out of range (locals: {method.Locals.Count})");
                    break;
                case DcilOp.LoadField or DcilOp.StoreField or DcilOp.Call or DcilOp.CallVirt or DcilOp.NewObj:
                    if (ins.Token is null) Err("missing metadata token");
                    break;
                case DcilOp.Conv:
                    if (ins.Type is null || !DcilType.NumericPrimitives.Contains(ins.Type)) Err($"conv needs numeric type, got '{ins.Type}'");
                    break;
                case DcilOp.NewArr or DcilOp.LoadElem or DcilOp.StoreElem or DcilOp.Cast or DcilOp.Box or DcilOp.Unbox:
                    if (!DcilType.IsValid(ins.Type)) Err($"invalid or missing type '{ins.Type}'");
                    break;
                case DcilOp.Br or DcilOp.BrTrue or DcilOp.BrFalse or DcilOp.Leave:
                    NeedTarget(ins.Target);
                    break;
                case DcilOp.Switch:
                    if (ins.Targets is null || ins.Targets.Length == 0) Err("switch without targets");
                    else foreach (var t in ins.Targets) NeedTarget(t);
                    break;
                case DcilOp.Unknown:
                    if (string.IsNullOrEmpty(ins.Tag)) Err("unknown without tag");
                    break;
            }
        }

        foreach (var r in method.Regions)
        {
            if (r.TryStart < 0 || r.TryStart >= r.TryEnd || r.TryEnd > n) errors.Add($"region: bad try range {r.TryStart}..{r.TryEnd}");
            if (r.HandlerStart < 0 || r.HandlerStart >= r.HandlerEnd || r.HandlerEnd > n) errors.Add($"region: bad handler range {r.HandlerStart}..{r.HandlerEnd}");
            if (r.Kind == DcilRegionKind.Catch && !DcilType.IsValid(r.CatchType)) errors.Add($"region: catch without valid type '{r.CatchType}'");
        }

        return errors;
    }
}
