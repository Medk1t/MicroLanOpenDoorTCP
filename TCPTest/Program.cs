using TCPTest;
 
    //Подъезд
    var podezd = new MicrolanPing(IP: "10.40.58.35");
    await podezd.Execute([0x42, 0x3, 0x1, 0x0, 0x1, 0x17]);

    //Улица
    var ulitsa = new MicrolanPing(IP: "10.40.58.36");
    await ulitsa.Execute([0x42, 0x3, 0x1, 0x0, 0x1, 0x17]);
