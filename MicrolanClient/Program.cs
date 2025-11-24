using MicrolanClient.Core;

//Подъезд
using MicrolanConnection podezd = await MicrolanConnection.CreateAsync("10.40.58.35");
await podezd.OpenDoorAsync();

//Улица
using MicrolanConnection ulitsa = await MicrolanConnection.CreateAsync("10.40.58.36");
await ulitsa.OpenDoorAsync();