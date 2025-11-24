using MicrolanClient;

//Подъезд
using var podezd = await MicrolanConnection.CreateAsync("10.40.58.35");
await podezd.OpenDoorAsync();

//Улица
using var ulitsa = await MicrolanConnection.CreateAsync("10.40.58.36");
await ulitsa.OpenDoorAsync();