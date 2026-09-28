// WireRenderer/PanelConstants rely on shared static state (WiringOffset, UsedX3Values,
// lst_WireCodes_Info_Processed) by design, so tests that exercise them must run sequentially.
[assembly: DoNotParallelize]
