ALTER TABLE [dbo].[ActivityLogs] ADD [ActionType] [nvarchar](max)
ALTER TABLE [dbo].[ActivityLogs] ADD [PageUrl] [nvarchar](max)
ALTER TABLE [dbo].[ActivityLogs] ADD [RemoteAddress] [nvarchar](max)
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201506230643104_Activitylog-changes', N'NITASA.Data.Migrations.Configuration',  0x1F8B0800000000000400ED5DDD6EDCBCD13E2FD07B10F6A82D52AF9DA47F81DDC2B19DC2F892D8C83A6F7B66C82BDA16AA95F693B4796D14BDB21EF4927A0BA5448A3FE25022254ABB6B0B01022F293EC39FE170382467FEFBEFFF1CFFE56915793F509A85497C323B3A389C79285E2641183F9CCC36F9FD6FFF38FBCB9F7FF98BE38B60F5E4FD547DF7AEF80E978CB393D9639EAF3FCCE7D9F211ADFCEC60152ED3244BEEF38365B29AFB41327F7B78F8A7F9D1D11C618819C6F2BCE36F9B380F57A8FC817F9E25F112ADF38D1F7D49021465341DE72C4A54EFABBF42D9DA5FA293D9D7CB9BD3C5E9C1B99FFB33EF340A7D5C85058AEE679E1FC749EEE7B8821FBE676891A749FCB058E3043FBA795E23FCDDBD1F658856FC03FFDCB40D876F8B36CC79C10A6AB9C9F264650978F48E76CABC5EBC53D7CE58A7E16EBBC0DD9B3F17AD2EBBEE6476BACCC31F38E973F230F3EAF43E9C4569F1ADD4B9074291379E90F186B101E696E2DF1BEF6C13E59B149DC46893A77EF4C6BBDEDC45E1F2FFD0F34DF20F149FC49B2812EB876B88F3A4049C749D266B94E6CFDFD03DADF5E5F9CC9BCBE5E6F582AC98508634E632CEDFBD9D795F3171FF2E426CF885862FF224457F45314AFD1C05D77E9EA3148FDE6580CA0E54A8D76861364BDBE93563DC8479842A08CCB478E2CDBC2FFED367143FE48F2733FCE7CCFB143EA1A04AA1B0DFE310CF535C284F37AD54CE51B64CC335E1AE81699D06010AAE181DCC33E8064F76EB9E291830890B90C1AB7CED3FA0EF6934389D6F6895E40877508AB2CC3DB5AFFE8FF0A1E46C805167DE37149599D963B82642539CE4B7E4A34F69B2FA9644B2CC28F36E17C9265D168391683EB8F1D30794CBD53A9E7361D428A2087933D9547C3B092598D65FBF736A8331F2C5CA0F9BA6CBD1E1A111154B9170ED67D9CF491A8C4FF953986679F16703E9DF0D42F9B3EF8A704BDFA6C97D18A1EB70F9FDDBE7C11968E147399E35FDD64E572B0DD63AC3FB108632A8C1C79ECDA8C8F7C5B9CC4A89CC38E56382C5B41F77C03947F73E169F2E80228485585FA062BDB156B4B4AB618106AE86E5224672F932C81295F58FE7F45AF80841B385AFF8765AF8605AA32C7C2D9278A0D5C7994A0D8AABA184A356AE0D22358CE7DB19AEFE43923E9BCEB9EAFB69DEBDBA79B788360F2F6A6F7EEDA778502B8E56B8E475AB5B5B164BC96A85C7C6582A91CF27A104D33A4BE21C7FD9D73A673637DF1EBDFFC3FB3FBEFBFDFB3F0C21840A35B345FC39120E7C0738822DCC9534B9CC8A4383924BFA6E331890BD8A44CAD94B34BC67BBDB64FDAB4E5078BD871BB692D037E467432C57DA3D1B9DCFE0B68D8AC25BF60DDFBCD5B2942D5C3DBFD7468ED13795E0E5E7930487698DA2568E72BAD076C4F37618E5D5AD5E69BB688D60433C4B7EA0944DBA81897D427E310D83CB95FF303CCB5C66C5D6E322081DAC6A975955F7BE48B4AFAFD2A038AAE9A3575DC4C5C74CE5ED5BAD12866006376114F5AB1C6DE64F21FA7937ECD36156CAFCECB1FF1032A0EEE61C57DBB65DDAA535A91E4BC20EDC6C04AA20D237B02A027EC2540EAE92C0DF55AA8B65B53FFB77286AAA73F9414385A57C5D6DE58F6CABBAD11D4F57809BDAD1B4980EE87442A60B85CEDA6028179B143C9896D60AD54D5CF785D10AEB214C52FA7D8E9594611F378899EA0FCD34513F84A64CF7AD5967B9D85A5F875B362A24ADA67759669ADB30ADB273A6896D374BEC5662CDFC80976BD35A1A6A0BF4334D0DC9FF8DF523FFF79ABA5673769AACDBB6B44C07783B65359FCEE01AD401B2F3B7D2068A22937C8169395AC78B2EEE74C4723DD885EB1AA58F69F273C64D63C3D1A1AC35C27DF52C8936A308B6ABC528679C578B61FACE58C47C4141E89B0A97F2E349ACC0B4F6496D69BB95933F8E7F4433CAD9D7698E91EF36F9F0942E9E8A4566AB3AD87EAA5CE35CEAB61090F1C65C3EC69B493CC2B446118FE43E61310C43BF837C3FC8FE7014198879A66FEF8CB38F0DB375E43F3B38DE9D76A9C34AC8DC4283CC27055243CBD1BE1437A9E54DE21E9A9936F96332FC46F62C45BA6B936EA6CAB7F0E131CF2E63AB3756429969EAC0B43A3DC8AB6394FD3CC0AEDF983B1628CF4B82668C413F9F7802A6358AFDE6273FDA6C8F5FA23030775240BEDE1AB7041DB8257861DB13472C697D6E1D8C605899546CB7D7FCCAE99A8167DF65D66D35FBF9A9B798AEDC8C9332A11B717662C74AEA4C4BD416850E19F27659DAC2E18EEE99EFA43BAA1BF434FC8DFDCF61FC8F576833717C6FA9927A5DC562FD1A1028333B89C5BF854159D84C2E92AFB7251809F52EE291971C4B48128AA3884A426A948D0321358E7823B4AEC631D55062FD05902BF74104C552A12385DA94509D6C38CDB2641996DC4FF1B82B1F99EC451C785ABF3E64A02A4F4078ACB03408D778FE639227B3DF286D80C0981AC8C188F149063B9AD585C6554C545A8FF83D2CAE82674B3F50BB1BF74120A76039838A7398D08FCEB084C6922B8C73552885F1325CFB91AECAB502860A5E5121065DCF39476B14176247D7E726342B5B934A97C1D73AA8AD3F8EE702C334F391E22051C7017A6F899C11242FACE6CCA5F5B3D8C2B0DBE7315DCD476035DD789890AEFCBA6E85E3B48F3C74ECD1FEE283B389F2A2C99C0B5BDF8B886434F8DBE7C8B6568CC0996DE3655205F1EDD46E70297DB560CC3DF5570CC3F068FDD5A242650F38546EC33618541E2B23FEE447ABDB644FF9454D0BD7689ED7282C431F9E5873A5E621EDFEB024D880F1F8111C9FBD6346C23D46AC527B47E59811E52758023A08BB634C28557E6C1694C6C584387B0DB94DF6238FE85B58A3F6A2BE553619709BFC0EBF65CB72787070A440F6611789FA788C22F5A3095966B5DD128BC81EB4F443AA71A725320AF5E962C328B013AEFD581AC1BA8FC269E058ECC382281D1CE8B8023E45E02C414F45CDD90C3EADAD01EEA01105AAB7117F293740ACF80BEA7F13B2FCF87304DE22E6E0829F71099456D73E6E16E71F4B267F82CE87F0C2438F88326A62AFB34C01BA40B96A039C79DC000D19F514E69391C892A74090E496B2C48AAC9425C92D65F95E5A29CFB3DA302AC1AE425439AD085488030834C70CA1A939F52FCC10A9CEAB83A3D92D583A10B3D2D2DB635D45486E0B127D62A860D0F4D6D2C5FB1BA07091DC5A368709E7ED74E5ABBA2A9F4BD92D58F4A66606E0B03B9F6D10A5180311E83A6102A02DDF5ABC3ADD568A5719B5F2828094E50D0D6821E42BD12EEA72BAE9908CD5B392668A946F3A15134A830339979B61D044358295DAD2E6431CA9CADA631CA1E60D12BF190BE80550F4DBF782DE3394DA1B66070C76470C428B5A24B0DD998288AB13E90E7AAB5A7E0C3A0BDA215959BA5D75556DB3A4C2BAEBA89A63226D2F35985BCD0DAE6A43C0F5D3DCC23A7CCFD00AB6F40B60F933B5FDF5EE13D9D827C0C1BA49E7FEA09E1AB53D0118A1DACD50ED03D86E771A42E4D6BC8E438D6E30AB181956A4A6835AB69125659029205FFF535BAFDFE5B7EFF3850A577A5443B3C19D7D1D2255301ADA5C5D34623B4B96773C27318969C2F1FC0C0E5E7CFCC55FAF0B1D9497A429DE8244323EFBEDC23ED2EF8A60CC971910F097D59651CA93D47F40B55CF242A18C68585C83BCF38B3B5D67C14AF94CDA476BD4D58A94BA5556C7ABD264AB32C5DF74C70E071E060C0EB4EC27DCAE82D5CB26228D72A694F68A88D27EE4A7C01DF4B324DAAC62BD694E5FBABA2E2222E8AE90E851E8D5441184269963484F44452429C31C8FDD4916B158A2058E10CA578212D2CDD158B45E118A259AE3D4A2F18A68B52C15F3785E6344C5D6A5B0BB621C946790D1FCD2EC5ECC261654D86046C1C586994AE4D6B1589EA49823D0C8B422044DB261B02ACCACCC6155AA3992103656841292CDB1782058118AA75AB44F0EF52A3552CE32C764015D45349638BEC811DFB5895062BA65AD3E3E03B582CEECDA6B558712D3CDD1F88D6D69C6B1541B2416F2558662C97658F46D5E1D8B265B88687AF75792CD9AFBC05B13CA1A1B9A9950860A1B0865B8D8AE0A65556CD98A2C679A88E389EC46BCB8160BF613704B53A7C1F668367D74000653485FF4E54E23E2474A5AA2CB94ED6D31D488A4B2CE55CF9D3489D72E32A81DACB3C480CB9B080C5DC961E485704B470469B8BCA3C7723D6B794CD2BAD1A3C75E44D9D2CB59E3CF7C29B4A83C2F840CAB39CBE38BD6262DCFE8800788269E61B59FA0C1486BFB099A6A31022C1CA934042CD516A98A37AAA255393B249F74C67E53F9049637924F9A92BBAACFA806415B53E02E1A4D159B8AA525450E6B298B7E31C7C20A25C7AE942C5172968DAC102354CAF242CCB141E4712A653C9E6EBD7AD277F1C0024A732C2C8B72CC4AC9C22867D9D4520D5D29D755CDB7EE0372690CE802E83659139EAB75558A6729624919163A8418D452D220C48C49E3DF5B8D5FBE26D37365ED63326843186827A0D91D77DB17BBDC576CCB2CB75D4ED4DDE4B1624310C49C0735C5876140F6804D3E79D2BC6AD3E34CACD799F5FAF15C57661B95CB2603E964C67C6D4A4DA982F75C49200CF385042EBDFB264D1E0E4E84E2A9165C730DDE011292CDB15808381189255AE0B0306512104BB5B9E5C463B8C9579C78BA395A15A64D44AAD26C50C0E609C93B334FC9A3A9AE33142C6D303735E55EEEBA4B42A0C9477F45CA98C644215699B4AAF1640B63118F4626198A78F2A401BC720DA07C51D95DB0A8858DE40A546C57C58A1C5D4CBD1750E58C7B5ED05FD0D08060F26A7C6FD712071B15C975B5B45391722641F5EA0555DE4703520B1B092AA8D8EEEF4A5830301189256ED18A40037A497380A659F4148FD725F5144FDE19A695FC057465DE261003266E2E3E0C33DBDD98D6A208B1B92424217D67869A797BE83ACC954F08FB11D6961C6670FBEFB368FC2CC9644292766738E96BCDCEA309BE3835194C4D41ED58D6D76E2B09BF0BBB6E12A94A5EF454DF244D0893CAF722553EFAE8BAD714EC3A03F7677FCA5D9DC9FB309D03B48696A837B3AC6F6439D9DB961ED3248832C5E2A8B88CBA249D1397292F615FBA2747C5D44354D7C90B173798BDBA82BAEEE4A18DC4FED4854A6A475227B4986E8BA6AEAE62BA2D1A3039A50C5BBC2B606B28E75823AAF34DCAB0BA46EEE8592A0F2524CD38966A8BA4C800963AF2E455FC8BD43F61D4690AFBCDFC8B50DF1E92D391B2F5850B91B2D519F5335277F6413E9979B88B7E144B155EB29EB31CADE8DAFDFFD1591496F758AB0FBEF871788FB29CC4409BBD3D3C7A3BF34EA3D0CF88AB17EAC6E443DDD3A6915F93A377855F1314ACE6F5E2F6DE510A942C0BA41B2B6AE038AD8B9031825C8645C7B64668B30CB955791B1148285E4F2FE3003D9DCCFE5916F9E05DFEFD96947AE39513FC8377E8FDAB5FB8C8F8879F2E1FFDF4572BFFE9D72254C7E8F6BDF06AE11403DCD179A7708A828B1207D5626E4A1C60D59C9458231A475A04025FEDEB4C11C31AF6EA7CEA54444692C2FB1D1D1E5A578FFB18710C2C781C6940FE9D3D30F73F628C6B344F643F240E068CF92211B8AB5F88D6CE32458DB90D41194BB98FBD1AA546E0EE82520F1E791776C160DE46FA814891C1BB80540673BBF59494B2584F8D053010C8F2D50BE056A1D3454C3AD31A1AE7A549F3DCC988D6096E02D2615A1973B726C2E1C4E1437038B925B2830ABBEAEAA333B3BECE757AEBD318728FB1B7B358B8FD61A705B082BD36D636B3CBD8440009946E7355F6CCB143DB75C9334777055072C8D1670DE76E38FA0910EE84A37BA3B8FB0D17032679DF18CEF6008724DB57A9E24C37706690022D789296F1B68396E15E7C39B202C82E341C00D61C68384094FD67F4D90973BF19DD51646F19DDD5979A8B8C3E15525D6274AF96E40963FB9621C90146F73E92FC5EB8D93D0EA3031BE853B4984E9B7A193BE026BF127BBBD869F672263A342BD94B89DEB20E6F201406D93BDAB21D14927A5F798E39A5B01B705A6CE2B601B9ED25B1D964E3DBEA2E7F32CCD94C3CADA387BD9D7EB0A8B504E1AE227A9A66AEBB5FC150D198AF081758CC9D8293DB26DC5F8403B8CA698413A8EEED349E44804786BD9D3E5B5BBDCCCE810ABF0F2E6C46CE6C64820708173787B80F887196D4ED6FF3B7742565B82555F5A3300903D92743F7716BB748BFB757899DC902EAA8A17BF3DC29E7D26B98EDDB2EF74B846C5B29571D1CECAD0471A28D331F09BBB86DA58E125C9C3F71570956D3CDFCAAA0D6E7C0DEF217701BD31641705B30DC16017406B0B7BDEE6C7B46FD0B0CD8EFC0BB7DC36E571EE91A747BB01FEA98D1F819CAF7C0D13E6AD234744C35A0A6013CAADF5BA1E46C76F047FA76673555B95E4735667745467E80471EFA3B0022EFFD5FE00667B8A331E8E5BCD91CD5BD966F9FA9BCE480F3557C80EF8023C417F8CEE0DC4D21F901BE3BC0BE13C0C5E32CFEE8BEF322C75FDBDB2CDABA49749A65C9322CE534C52FAE38DF02FB9E8B38F08A64FA8E8A522F1EAE93E8CBDE974D9487EB285C623227B323C573C6554C56678F3C062E6EB6644B5F7594545435D0D1FE2E9CEA94B449824CFB370A249EEAA8B0AB857E84F7D9599EFAA1EA8AE03A0DE365B8F623B19DB58F0C97F8A2150CAE9E738ED6282E6481D826133A3A0F74C7730659EBC9B6761FCF050668E60BE1BDFF2DF094D9728806620FD129815805297D1066311EC49ECCA275BBA021573935D80ACFD46EE4DD6AAEE6F1F1631F8883C71347E1A1FA3542A92AF5BC4178A921B696737E6A0FE7A5926C8A98B51DBE029F3728430A0DE5EB612A6D44D4DDE029BD9FE03159AABC5EB73FFC446E0302D5A0192F8493804B8FFBC046D05D4D3E84EAD88183F60219C87C38C7669FCFBA687C6332CFEDA6B77A7D787070A48C5527F9B567EAB18D6CD2FA9C1C69B0CB17587BB1D648CFCD680D68DAFEAF30C07B7B0DA96D2F2EA5D9FC163CA8E3A345B3C5C1AA9246E1167250A2901F8853B46ECA4D8E29AD1845E78D5925A4775D3C00975CC81E3085C346C95BA5C22ED4A9A66ADC9979DC5208594F88DFCB93597097E05126F646E10325CE9F428BAC570A11920CA11739EDB0C408A9C0926408B6C86987E5BB32059A6741F0343734A15109539544950352209926F874B900F0690E8C5F661AE3377555FD8B067A561D27EAAA3AAA34BB8164F9453B391D9D060286C8D2036A5D3B486E43338A0FDA69913BF82A159A0EE1D3AC56E07803E216C9306CBC31A96F0E5737D7D636F70DE48578C50B901B5236283F842FDAC955D79A544A2C0722C2A2B2B4E2D3355E85A719203A0D126204AEC36E806E47A667B62A74950161933C155C5822E5E5869C677942BEB0E800875D924223320809489440BEC6952D9A50E23B10B7565AD1F157064D518E608016351FD3D857532A01A809654129BD7733B5A7064073CD4E1834670C420B786243F335EB9C18075A87E3A21BAAB5DAA017E0FD13B4AB549BB07B5D201B65F5ED6F30DEBA6CBCA40688856986DB6653D0964683262ED5D828D419ACEC369B4BCC5EFA860266B19EE2CC820F3A354B36F0802D6BB001396259696B41CBD1B4DE4D940C1240FBF4060BD06421D4B14A6A689AA49408818FCC9BA5C48F6079C773A277D004FC53891381F7EC9BB8B878437E9DA32C7CE010C71833464B69B7CEBEB98CEF93CA6C50AB51F549FD7A2C566903BC953F4DF3F0DE5FE6387B89B2AC541BCB9BE1852BF53B145CC6579B7CBDC97193D1EA2E926470617C68A27F3C57EA7C4C6E61652E9A80AB19167795AEE28F9B300A58BD3F0137FE34108555835EAB2BC6322FAED73D3C33A4AF496C0844BB8F19636ED06A1D61B0EC2A5EF8C50D2FFBBA6161F3193DF8CBE76B1AEE430FD23E1072B71F9F87FE43EAAF328AC1CBE39F988783D5D39FFF07BBCA7F1F86220100 , N'6.1.3-40302')
