using Goldman.Models.Devices;

namespace Goldman.Http.Requests;

public record CreateDeviceRequest(string Name, DeviceType Type);