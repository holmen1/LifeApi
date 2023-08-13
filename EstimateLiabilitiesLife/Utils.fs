namespace EstimateLiabilitiesLife

open System
open System.Collections.Generic

module Time =

    /// Calculates the number of years between two dates
    let years (startDate: DateTime) (endDate: DateTime) : float =
        (endDate - startDate).TotalDays / 365.25

    let monthsBetweenDates (startDate: DateTime) (endDate: DateTime) : int =
        12 * (endDate.Year - startDate.Year) + endDate.Month - startDate.Month

    type T =
        { startDate: DateTime
          months: int }

        member this.Months = [ 0 .. this.months ]
        member this.Dates = this.Months |> List.map this.startDate.AddMonths
        member this.Years = this.Dates |> List.map (fun d -> years this.Dates.Head d)

module Simpson =
    // Simpson's rule for numerical integration
    let compositeSimpson n a b f =
        if n % 2 <> 0 then
            invalidArg "n" "must be even"

        let h = (b - a) / float n
        let sumOdd = seq { 1..2 .. n - 1 } |> Seq.sumBy (fun i -> f (a + float i * h))
        let sumEven = seq { 2..2 .. n - 2 } |> Seq.sumBy (fun i -> f (a + float i * h))

        h / 3.0 * (f a + 4.0 * sumOdd + 2.0 * sumEven + f b)


module Rate =

    // format from pandas.DataFrame.to_json(orient='index')
    let private json =
        """{"0":{"spot":0.03015},"1":{"spot":0.03015},"2":{"spot":0.03015},"3":{"spot":0.03015},"4":{"spot":0.03015},"5":{"spot":0.03015},"6":{"spot":0.03015},"7":{"spot":0.03015},"8":{"spot":0.03015},"9":{"spot":0.03015},"10":{"spot":0.03015},"11":{"spot":0.03015},"12":{"spot":0.03015},"13":{"spot":0.029957428},"14":{"spot":0.029764856},"15":{"spot":0.029572284},"16":{"spot":0.029379712},"17":{"spot":0.02918714},"18":{"spot":0.028994568},"19":{"spot":0.028801996},"20":{"spot":0.028609424},"21":{"spot":0.028416852},"22":{"spot":0.02822428},"23":{"spot":0.028031708},"24":{"spot":0.027839136},"25":{"spot":0.027638694},"26":{"spot":0.027438253},"27":{"spot":0.027237811},"28":{"spot":0.02703737},"29":{"spot":0.026836928},"30":{"spot":0.026636487},"31":{"spot":0.026436045},"32":{"spot":0.026235603},"33":{"spot":0.026035162},"34":{"spot":0.02583472},"35":{"spot":0.025634279},"36":{"spot":0.025433837},"37":{"spot":0.025307936},"38":{"spot":0.025182035},"39":{"spot":0.025056134},"40":{"spot":0.024930233},"41":{"spot":0.024804332},"42":{"spot":0.024678431},"43":{"spot":0.02455253},"44":{"spot":0.024426629},"45":{"spot":0.024300728},"46":{"spot":0.024174827},"47":{"spot":0.024048927},"48":{"spot":0.023923026},"49":{"spot":0.023837581},"50":{"spot":0.023752137},"51":{"spot":0.023666693},"52":{"spot":0.023581248},"53":{"spot":0.023495804},"54":{"spot":0.02341036},"55":{"spot":0.023324915},"56":{"spot":0.023239471},"57":{"spot":0.023154027},"58":{"spot":0.023068583},"59":{"spot":0.022983138},"60":{"spot":0.022897694},"61":{"spot":0.022844493},"62":{"spot":0.022791291},"63":{"spot":0.02273809},"64":{"spot":0.022684889},"65":{"spot":0.022631688},"66":{"spot":0.022578487},"67":{"spot":0.022525285},"68":{"spot":0.022472084},"69":{"spot":0.022418883},"70":{"spot":0.022365682},"71":{"spot":0.02231248},"72":{"spot":0.022259279},"73":{"spot":0.022227684},"74":{"spot":0.022196089},"75":{"spot":0.022164493},"76":{"spot":0.022132898},"77":{"spot":0.022101303},"78":{"spot":0.022069707},"79":{"spot":0.022038112},"80":{"spot":0.022006517},"81":{"spot":0.021974922},"82":{"spot":0.021943326},"83":{"spot":0.021911731},"84":{"spot":0.021880136},"85":{"spot":0.021854737},"86":{"spot":0.021829338},"87":{"spot":0.021803939},"88":{"spot":0.02177854},"89":{"spot":0.021753141},"90":{"spot":0.021727742},"91":{"spot":0.021702343},"92":{"spot":0.021676944},"93":{"spot":0.021651545},"94":{"spot":0.021626146},"95":{"spot":0.021600747},"96":{"spot":0.021575347},"97":{"spot":0.021558638},"98":{"spot":0.021541928},"99":{"spot":0.021525218},"100":{"spot":0.021508508},"101":{"spot":0.021491798},"102":{"spot":0.021475088},"103":{"spot":0.021458378},"104":{"spot":0.021441669},"105":{"spot":0.021424959},"106":{"spot":0.021408249},"107":{"spot":0.021391539},"108":{"spot":0.021374829},"109":{"spot":0.021367012},"110":{"spot":0.021359195},"111":{"spot":0.021351377},"112":{"spot":0.02134356},"113":{"spot":0.021335743},"114":{"spot":0.021327926},"115":{"spot":0.021320109},"116":{"spot":0.021312291},"117":{"spot":0.021304474},"118":{"spot":0.021296657},"119":{"spot":0.02128884},"120":{"spot":0.021281023},"121":{"spot":0.021288061},"122":{"spot":0.021295099},"123":{"spot":0.021302137},"124":{"spot":0.021309175},"125":{"spot":0.021316213},"126":{"spot":0.021323251},"127":{"spot":0.02133029},"128":{"spot":0.021337328},"129":{"spot":0.021344366},"130":{"spot":0.021351404},"131":{"spot":0.021358442},"132":{"spot":0.02136548},"133":{"spot":0.021382485},"134":{"spot":0.021399491},"135":{"spot":0.021416496},"136":{"spot":0.021433501},"137":{"spot":0.021450506},"138":{"spot":0.021467511},"139":{"spot":0.021484516},"140":{"spot":0.021501521},"141":{"spot":0.021518526},"142":{"spot":0.021535531},"143":{"spot":0.021552536},"144":{"spot":0.021569541},"145":{"spot":0.021587483},"146":{"spot":0.021605426},"147":{"spot":0.021623368},"148":{"spot":0.02164131},"149":{"spot":0.021659252},"150":{"spot":0.021677194},"151":{"spot":0.021695136},"152":{"spot":0.021713078},"153":{"spot":0.02173102},"154":{"spot":0.021748962},"155":{"spot":0.021766904},"156":{"spot":0.021784846},"157":{"spot":0.02181054},"158":{"spot":0.021836234},"159":{"spot":0.021861928},"160":{"spot":0.021887622},"161":{"spot":0.021913316},"162":{"spot":0.021939011},"163":{"spot":0.021964705},"164":{"spot":0.021990399},"165":{"spot":0.022016093},"166":{"spot":0.022041787},"167":{"spot":0.022067481},"168":{"spot":0.022093175},"169":{"spot":0.022125062},"170":{"spot":0.022156948},"171":{"spot":0.022188835},"172":{"spot":0.022220722},"173":{"spot":0.022252608},"174":{"spot":0.022284495},"175":{"spot":0.022316381},"176":{"spot":0.022348268},"177":{"spot":0.022380154},"178":{"spot":0.022412041},"179":{"spot":0.022443928},"180":{"spot":0.022475814},"181":{"spot":0.022506291},"182":{"spot":0.022536767},"183":{"spot":0.022567243},"184":{"spot":0.02259772},"185":{"spot":0.022628196},"186":{"spot":0.022658673},"187":{"spot":0.022689149},"188":{"spot":0.022719625},"189":{"spot":0.022750102},"190":{"spot":0.022780578},"191":{"spot":0.022811055},"192":{"spot":0.022841531},"193":{"spot":0.022878111},"194":{"spot":0.02291469},"195":{"spot":0.02295127},"196":{"spot":0.02298785},"197":{"spot":0.023024429},"198":{"spot":0.023061009},"199":{"spot":0.023097589},"200":{"spot":0.023134168},"201":{"spot":0.023170748},"202":{"spot":0.023207328},"203":{"spot":0.023243907},"204":{"spot":0.023280487},"205":{"spot":0.023322144},"206":{"spot":0.023363801},"207":{"spot":0.023405458},"208":{"spot":0.023447116},"209":{"spot":0.023488773},"210":{"spot":0.02353043},"211":{"spot":0.023572087},"212":{"spot":0.023613745},"213":{"spot":0.023655402},"214":{"spot":0.023697059},"215":{"spot":0.023738716},"216":{"spot":0.023780373},"217":{"spot":0.023826299},"218":{"spot":0.023872225},"219":{"spot":0.023918151},"220":{"spot":0.023964077},"221":{"spot":0.024010003},"222":{"spot":0.024055929},"223":{"spot":0.024101855},"224":{"spot":0.024147781},"225":{"spot":0.024193707},"226":{"spot":0.024239632},"227":{"spot":0.024285558},"228":{"spot":0.024331484},"229":{"spot":0.024381032},"230":{"spot":0.02443058},"231":{"spot":0.024480129},"232":{"spot":0.024529677},"233":{"spot":0.024579225},"234":{"spot":0.024628773},"235":{"spot":0.024678321},"236":{"spot":0.024727869},"237":{"spot":0.024777417},"238":{"spot":0.024826965},"239":{"spot":0.024876513},"240":{"spot":0.024926061},"241":{"spot":0.024978709},"242":{"spot":0.025031356},"243":{"spot":0.025084004},"244":{"spot":0.025136651},"245":{"spot":0.025189298},"246":{"spot":0.025241946},"247":{"spot":0.025294593},"248":{"spot":0.02534724},"249":{"spot":0.025399888},"250":{"spot":0.025452535},"251":{"spot":0.025505183},"252":{"spot":0.02555783},"253":{"spot":0.025605719},"254":{"spot":0.025653608},"255":{"spot":0.025701498},"256":{"spot":0.025749387},"257":{"spot":0.025797276},"258":{"spot":0.025845166},"259":{"spot":0.025893055},"260":{"spot":0.025940944},"261":{"spot":0.025988834},"262":{"spot":0.026036723},"263":{"spot":0.026084612},"264":{"spot":0.026132502},"265":{"spot":0.02617625},"266":{"spot":0.026219998},"267":{"spot":0.026263747},"268":{"spot":0.026307495},"269":{"spot":0.026351243},"270":{"spot":0.026394992},"271":{"spot":0.02643874},"272":{"spot":0.026482489},"273":{"spot":0.026526237},"274":{"spot":0.026569985},"275":{"spot":0.026613734},"276":{"spot":0.026657482},"277":{"spot":0.026697604},"278":{"spot":0.026737727},"279":{"spot":0.026777849},"280":{"spot":0.026817971},"281":{"spot":0.026858093},"282":{"spot":0.026898216},"283":{"spot":0.026938338},"284":{"spot":0.02697846},"285":{"spot":0.027018582},"286":{"spot":0.027058705},"287":{"spot":0.027098827},"288":{"spot":0.027138949},"289":{"spot":0.027175878},"290":{"spot":0.027212807},"291":{"spot":0.027249736},"292":{"spot":0.027286665},"293":{"spot":0.027323594},"294":{"spot":0.027360523},"295":{"spot":0.027397452},"296":{"spot":0.027434381},"297":{"spot":0.02747131},"298":{"spot":0.02750824},"299":{"spot":0.027545169},"300":{"spot":0.027582098},"301":{"spot":0.0276162},"302":{"spot":0.027650302},"303":{"spot":0.027684405},"304":{"spot":0.027718507},"305":{"spot":0.02775261},"306":{"spot":0.027786712},"307":{"spot":0.027820815},"308":{"spot":0.027854917},"309":{"spot":0.02788902},"310":{"spot":0.027923122},"311":{"spot":0.027957224},"312":{"spot":0.027991327},"313":{"spot":0.028022915},"314":{"spot":0.028054504},"315":{"spot":0.028086092},"316":{"spot":0.02811768},"317":{"spot":0.028149269},"318":{"spot":0.028180857},"319":{"spot":0.028212446},"320":{"spot":0.028244034},"321":{"spot":0.028275622},"322":{"spot":0.028307211},"323":{"spot":0.028338799},"324":{"spot":0.028370388},"325":{"spot":0.02839973},"326":{"spot":0.028429072},"327":{"spot":0.028458415},"328":{"spot":0.028487757},"329":{"spot":0.0285171},"330":{"spot":0.028546442},"331":{"spot":0.028575785},"332":{"spot":0.028605127},"333":{"spot":0.02863447},"334":{"spot":0.028663812},"335":{"spot":0.028693155},"336":{"spot":0.028722497},"337":{"spot":0.028749825},"338":{"spot":0.028777153},"339":{"spot":0.028804481},"340":{"spot":0.028831809},"341":{"spot":0.028859136},"342":{"spot":0.028886464},"343":{"spot":0.028913792},"344":{"spot":0.02894112},"345":{"spot":0.028968448},"346":{"spot":0.028995776},"347":{"spot":0.029023104},"348":{"spot":0.029050431},"349":{"spot":0.029075945},"350":{"spot":0.029101459},"351":{"spot":0.029126973},"352":{"spot":0.029152487},"353":{"spot":0.029178001},"354":{"spot":0.029203514},"355":{"spot":0.029229028},"356":{"spot":0.029254542},"357":{"spot":0.029280056},"358":{"spot":0.02930557},"359":{"spot":0.029331083},"360":{"spot":0.029356597},"361":{"spot":0.029380472},"362":{"spot":0.029404347},"363":{"spot":0.029428221},"364":{"spot":0.029452096},"365":{"spot":0.02947597},"366":{"spot":0.029499845},"367":{"spot":0.02952372},"368":{"spot":0.029547594},"369":{"spot":0.029571469},"370":{"spot":0.029595344},"371":{"spot":0.029619218},"372":{"spot":0.029643093},"373":{"spot":0.029665481},"374":{"spot":0.02968787},"375":{"spot":0.029710258},"376":{"spot":0.029732647},"377":{"spot":0.029755035},"378":{"spot":0.029777424},"379":{"spot":0.029799812},"380":{"spot":0.029822201},"381":{"spot":0.029844589},"382":{"spot":0.029866977},"383":{"spot":0.029889366},"384":{"spot":0.029911754},"385":{"spot":0.029932791},"386":{"spot":0.029953828},"387":{"spot":0.029974865},"388":{"spot":0.029995902},"389":{"spot":0.030016939},"390":{"spot":0.030037976},"391":{"spot":0.030059013},"392":{"spot":0.03008005},"393":{"spot":0.030101086},"394":{"spot":0.030122123},"395":{"spot":0.03014316},"396":{"spot":0.030164197},"397":{"spot":0.030184001},"398":{"spot":0.030203805},"399":{"spot":0.030223609},"400":{"spot":0.030243414},"401":{"spot":0.030263218},"402":{"spot":0.030283022},"403":{"spot":0.030302826},"404":{"spot":0.03032263},"405":{"spot":0.030342434},"406":{"spot":0.030362238},"407":{"spot":0.030382042},"408":{"spot":0.030401847},"409":{"spot":0.030420523},"410":{"spot":0.0304392},"411":{"spot":0.030457876},"412":{"spot":0.030476553},"413":{"spot":0.03049523},"414":{"spot":0.030513906},"415":{"spot":0.030532583},"416":{"spot":0.03055126},"417":{"spot":0.030569936},"418":{"spot":0.030588613},"419":{"spot":0.03060729},"420":{"spot":0.030625966},"421":{"spot":0.030643609},"422":{"spot":0.030661252},"423":{"spot":0.030678894},"424":{"spot":0.030696537},"425":{"spot":0.03071418},"426":{"spot":0.030731823},"427":{"spot":0.030749465},"428":{"spot":0.030767108},"429":{"spot":0.030784751},"430":{"spot":0.030802394},"431":{"spot":0.030820036},"432":{"spot":0.030837679},"433":{"spot":0.030854372},"434":{"spot":0.030871064},"435":{"spot":0.030887756},"436":{"spot":0.030904449},"437":{"spot":0.030921141},"438":{"spot":0.030937834},"439":{"spot":0.030954526},"440":{"spot":0.030971219},"441":{"spot":0.030987911},"442":{"spot":0.031004603},"443":{"spot":0.031021296},"444":{"spot":0.031037988},"445":{"spot":0.031053805},"446":{"spot":0.031069622},"447":{"spot":0.031085439},"448":{"spot":0.031101256},"449":{"spot":0.031117072},"450":{"spot":0.031132889},"451":{"spot":0.031148706},"452":{"spot":0.031164523},"453":{"spot":0.03118034},"454":{"spot":0.031196157},"455":{"spot":0.031211974},"456":{"spot":0.03122779},"457":{"spot":0.031242799},"458":{"spot":0.031257807},"459":{"spot":0.031272816},"460":{"spot":0.031287824},"461":{"spot":0.031302832},"462":{"spot":0.031317841},"463":{"spot":0.031332849},"464":{"spot":0.031347858},"465":{"spot":0.031362866},"466":{"spot":0.031377874},"467":{"spot":0.031392883},"468":{"spot":0.031407891},"469":{"spot":0.031422152},"470":{"spot":0.031436412},"471":{"spot":0.031450673},"472":{"spot":0.031464933},"473":{"spot":0.031479193},"474":{"spot":0.031493454},"475":{"spot":0.031507714},"476":{"spot":0.031521975},"477":{"spot":0.031536235},"478":{"spot":0.031550495},"479":{"spot":0.031564756},"480":{"spot":0.031579016},"481":{"spot":0.031592583},"482":{"spot":0.03160615},"483":{"spot":0.031619717},"484":{"spot":0.031633284},"485":{"spot":0.031646851},"486":{"spot":0.031660418},"487":{"spot":0.031673985},"488":{"spot":0.031687552},"489":{"spot":0.031701119},"490":{"spot":0.031714686},"491":{"spot":0.031728253},"492":{"spot":0.03174182},"493":{"spot":0.031754743},"494":{"spot":0.031767666},"495":{"spot":0.031780589},"496":{"spot":0.031793511},"497":{"spot":0.031806434},"498":{"spot":0.031819357},"499":{"spot":0.03183228},"500":{"spot":0.031845203},"501":{"spot":0.031858126},"502":{"spot":0.031871049},"503":{"spot":0.031883972},"504":{"spot":0.031896895},"505":{"spot":0.031909218},"506":{"spot":0.031921542},"507":{"spot":0.031933866},"508":{"spot":0.031946189},"509":{"spot":0.031958513},"510":{"spot":0.031970837},"511":{"spot":0.03198316},"512":{"spot":0.031995484},"513":{"spot":0.032007807},"514":{"spot":0.032020131},"515":{"spot":0.032032455},"516":{"spot":0.032044778},"517":{"spot":0.032056543},"518":{"spot":0.032068309},"519":{"spot":0.032080074},"520":{"spot":0.032091839},"521":{"spot":0.032103604},"522":{"spot":0.032115369},"523":{"spot":0.032127134},"524":{"spot":0.032138899},"525":{"spot":0.032150664},"526":{"spot":0.03216243},"527":{"spot":0.032174195},"528":{"spot":0.03218596},"529":{"spot":0.032197204},"530":{"spot":0.032208447},"531":{"spot":0.032219691},"532":{"spot":0.032230935},"533":{"spot":0.032242178},"534":{"spot":0.032253422},"535":{"spot":0.032264666},"536":{"spot":0.03227591},"537":{"spot":0.032287153},"538":{"spot":0.032298397},"539":{"spot":0.032309641},"540":{"spot":0.032320884},"541":{"spot":0.032331641},"542":{"spot":0.032342397},"543":{"spot":0.032353153},"544":{"spot":0.032363909},"545":{"spot":0.032374666},"546":{"spot":0.032385422},"547":{"spot":0.032396178},"548":{"spot":0.032406934},"549":{"spot":0.032417691},"550":{"spot":0.032428447},"551":{"spot":0.032439203},"552":{"spot":0.032449959},"553":{"spot":0.032460259},"554":{"spot":0.032470559},"555":{"spot":0.032480859},"556":{"spot":0.032491158},"557":{"spot":0.032501458},"558":{"spot":0.032511758},"559":{"spot":0.032522058},"560":{"spot":0.032532357},"561":{"spot":0.032542657},"562":{"spot":0.032552957},"563":{"spot":0.032563257},"564":{"spot":0.032573557},"565":{"spot":0.032583428},"566":{"spot":0.0325933},"567":{"spot":0.032603172},"568":{"spot":0.032613044},"569":{"spot":0.032622915},"570":{"spot":0.032632787},"571":{"spot":0.032642659},"572":{"spot":0.032652531},"573":{"spot":0.032662403},"574":{"spot":0.032672274},"575":{"spot":0.032682146},"576":{"spot":0.032692018},"577":{"spot":0.032701488},"578":{"spot":0.032710958},"579":{"spot":0.032720428},"580":{"spot":0.032729898},"581":{"spot":0.032739367},"582":{"spot":0.032748837},"583":{"spot":0.032758307},"584":{"spot":0.032767777},"585":{"spot":0.032777247},"586":{"spot":0.032786717},"587":{"spot":0.032796187},"588":{"spot":0.032805657},"589":{"spot":0.032814749},"590":{"spot":0.032823841},"591":{"spot":0.032832933},"592":{"spot":0.032842025},"593":{"spot":0.032851117},"594":{"spot":0.032860209},"595":{"spot":0.032869301},"596":{"spot":0.032878393},"597":{"spot":0.032887486},"598":{"spot":0.032896578},"599":{"spot":0.03290567},"600":{"spot":0.032914762},"601":{"spot":0.032923498},"602":{"spot":0.032932235},"603":{"spot":0.032940971},"604":{"spot":0.032949708},"605":{"spot":0.032958444},"606":{"spot":0.03296718},"607":{"spot":0.032975917},"608":{"spot":0.032984653},"609":{"spot":0.03299339},"610":{"spot":0.033002126},"611":{"spot":0.033010863},"612":{"spot":0.033019599},"613":{"spot":0.033028},"614":{"spot":0.033036401},"615":{"spot":0.033044803},"616":{"spot":0.033053204},"617":{"spot":0.033061605},"618":{"spot":0.033070006},"619":{"spot":0.033078408},"620":{"spot":0.033086809},"621":{"spot":0.03309521},"622":{"spot":0.033103611},"623":{"spot":0.033112013},"624":{"spot":0.033120414},"625":{"spot":0.033128499},"626":{"spot":0.033136584},"627":{"spot":0.033144669},"628":{"spot":0.033152754},"629":{"spot":0.033160839},"630":{"spot":0.033168924},"631":{"spot":0.033177009},"632":{"spot":0.033185094},"633":{"spot":0.033193179},"634":{"spot":0.033201264},"635":{"spot":0.033209349},"636":{"spot":0.033217434},"637":{"spot":0.03322522},"638":{"spot":0.033233006},"639":{"spot":0.033240793},"640":{"spot":0.033248579},"641":{"spot":0.033256365},"642":{"spot":0.033264152},"643":{"spot":0.033271938},"644":{"spot":0.033279724},"645":{"spot":0.03328751},"646":{"spot":0.033295297},"647":{"spot":0.033303083},"648":{"spot":0.033310869},"649":{"spot":0.033318373},"650":{"spot":0.033325877},"651":{"spot":0.03333338},"652":{"spot":0.033340884},"653":{"spot":0.033348388},"654":{"spot":0.033355892},"655":{"spot":0.033363396},"656":{"spot":0.033370899},"657":{"spot":0.033378403},"658":{"spot":0.033385907},"659":{"spot":0.033393411},"660":{"spot":0.033400915},"661":{"spot":0.033408151},"662":{"spot":0.033415387},"663":{"spot":0.033422624},"664":{"spot":0.03342986},"665":{"spot":0.033437097},"666":{"spot":0.033444333},"667":{"spot":0.03345157},"668":{"spot":0.033458806},"669":{"spot":0.033466042},"670":{"spot":0.033473279},"671":{"spot":0.033480515},"672":{"spot":0.033487752},"673":{"spot":0.033494735},"674":{"spot":0.033501718},"675":{"spot":0.033508701},"676":{"spot":0.033515684},"677":{"spot":0.033522667},"678":{"spot":0.03352965},"679":{"spot":0.033536633},"680":{"spot":0.033543616},"681":{"spot":0.033550599},"682":{"spot":0.033557582},"683":{"spot":0.033564565},"684":{"spot":0.033571549},"685":{"spot":0.033578291},"686":{"spot":0.033585034},"687":{"spot":0.033591777},"688":{"spot":0.03359852},"689":{"spot":0.033605263},"690":{"spot":0.033612005},"691":{"spot":0.033618748},"692":{"spot":0.033625491},"693":{"spot":0.033632234},"694":{"spot":0.033638977},"695":{"spot":0.03364572},"696":{"spot":0.033652462},"697":{"spot":0.033658977},"698":{"spot":0.033665492},"699":{"spot":0.033672007},"700":{"spot":0.033678521},"701":{"spot":0.033685036},"702":{"spot":0.033691551},"703":{"spot":0.033698066},"704":{"spot":0.03370458},"705":{"spot":0.033711095},"706":{"spot":0.03371761},"707":{"spot":0.033724125},"708":{"spot":0.033730639},"709":{"spot":0.033736937},"710":{"spot":0.033743236},"711":{"spot":0.033749534},"712":{"spot":0.033755832},"713":{"spot":0.03376213},"714":{"spot":0.033768428},"715":{"spot":0.033774726},"716":{"spot":0.033781024},"717":{"spot":0.033787322},"718":{"spot":0.03379362},"719":{"spot":0.033799918},"720":{"spot":0.033806216},"721":{"spot":0.033812308},"722":{"spot":0.0338184},"723":{"spot":0.033824492},"724":{"spot":0.033830584},"725":{"spot":0.033836676},"726":{"spot":0.033842768},"727":{"spot":0.03384886},"728":{"spot":0.033854952},"729":{"spot":0.033861044},"730":{"spot":0.033867136},"731":{"spot":0.033873228},"732":{"spot":0.03387932},"733":{"spot":0.033885216},"734":{"spot":0.033891112},"735":{"spot":0.033897008},"736":{"spot":0.033902904},"737":{"spot":0.0339088},"738":{"spot":0.033914695},"739":{"spot":0.033920591},"740":{"spot":0.033926487},"741":{"spot":0.033932383},"742":{"spot":0.033938279},"743":{"spot":0.033944175},"744":{"spot":0.033950071},"745":{"spot":0.03395578},"746":{"spot":0.033961489},"747":{"spot":0.033967198},"748":{"spot":0.033972907},"749":{"spot":0.033978616},"750":{"spot":0.033984325},"751":{"spot":0.033990035},"752":{"spot":0.033995744},"753":{"spot":0.034001453},"754":{"spot":0.034007162},"755":{"spot":0.034012871},"756":{"spot":0.03401858},"757":{"spot":0.034024111},"758":{"spot":0.034029642},"759":{"spot":0.034035173},"760":{"spot":0.034040704},"761":{"spot":0.034046235},"762":{"spot":0.034051766},"763":{"spot":0.034057297},"764":{"spot":0.034062829},"765":{"spot":0.03406836},"766":{"spot":0.034073891},"767":{"spot":0.034079422},"768":{"spot":0.034084953},"769":{"spot":0.034090314},"770":{"spot":0.034095675},"771":{"spot":0.034101036},"772":{"spot":0.034106398},"773":{"spot":0.034111759},"774":{"spot":0.03411712},"775":{"spot":0.034122481},"776":{"spot":0.034127842},"777":{"spot":0.034133204},"778":{"spot":0.034138565},"779":{"spot":0.034143926},"780":{"spot":0.034149287},"781":{"spot":0.034154486},"782":{"spot":0.034159685},"783":{"spot":0.034164884},"784":{"spot":0.034170084},"785":{"spot":0.034175283},"786":{"spot":0.034180482},"787":{"spot":0.034185681},"788":{"spot":0.03419088},"789":{"spot":0.034196079},"790":{"spot":0.034201278},"791":{"spot":0.034206477},"792":{"spot":0.034211676},"793":{"spot":0.03421672},"794":{"spot":0.034221764},"795":{"spot":0.034226809},"796":{"spot":0.034231853},"797":{"spot":0.034236897},"798":{"spot":0.034241941},"799":{"spot":0.034246985},"800":{"spot":0.034252029},"801":{"spot":0.034257073},"802":{"spot":0.034262118},"803":{"spot":0.034267162},"804":{"spot":0.034272206},"805":{"spot":0.034277102},"806":{"spot":0.034281998},"807":{"spot":0.034286894},"808":{"spot":0.03429179},"809":{"spot":0.034296686},"810":{"spot":0.034301583},"811":{"spot":0.034306479},"812":{"spot":0.034311375},"813":{"spot":0.034316271},"814":{"spot":0.034321167},"815":{"spot":0.034326063},"816":{"spot":0.034330959},"817":{"spot":0.034335714},"818":{"spot":0.034340468},"819":{"spot":0.034345222},"820":{"spot":0.034349977},"821":{"spot":0.034354731},"822":{"spot":0.034359486},"823":{"spot":0.03436424},"824":{"spot":0.034368995},"825":{"spot":0.034373749},"826":{"spot":0.034378503},"827":{"spot":0.034383258},"828":{"spot":0.034388012},"829":{"spot":0.034392631},"830":{"spot":0.03439725},"831":{"spot":0.034401869},"832":{"spot":0.034406488},"833":{"spot":0.034411107},"834":{"spot":0.034415725},"835":{"spot":0.034420344},"836":{"spot":0.034424963},"837":{"spot":0.034429582},"838":{"spot":0.034434201},"839":{"spot":0.03443882},"840":{"spot":0.034443439},"841":{"spot":0.034447927},"842":{"spot":0.034452416},"843":{"spot":0.034456905},"844":{"spot":0.034461394},"845":{"spot":0.034465883},"846":{"spot":0.034470372},"847":{"spot":0.034474861},"848":{"spot":0.03447935},"849":{"spot":0.034483839},"850":{"spot":0.034488328},"851":{"spot":0.034492817},"852":{"spot":0.034497306},"853":{"spot":0.034501671},"854":{"spot":0.034506035},"855":{"spot":0.0345104},"856":{"spot":0.034514764},"857":{"spot":0.034519129},"858":{"spot":0.034523493},"859":{"spot":0.034527858},"860":{"spot":0.034532222},"861":{"spot":0.034536587},"862":{"spot":0.034540951},"863":{"spot":0.034545316},"864":{"spot":0.03454968},"865":{"spot":0.034553925},"866":{"spot":0.034558171},"867":{"spot":0.034562416},"868":{"spot":0.034566661},"869":{"spot":0.034570906},"870":{"spot":0.034575151},"871":{"spot":0.034579396},"872":{"spot":0.034583641},"873":{"spot":0.034587887},"874":{"spot":0.034592132},"875":{"spot":0.034596377},"876":{"spot":0.034600622},"877":{"spot":0.034604753},"878":{"spot":0.034608883},"879":{"spot":0.034613014},"880":{"spot":0.034617144},"881":{"spot":0.034621275},"882":{"spot":0.034625406},"883":{"spot":0.034629536},"884":{"spot":0.034633667},"885":{"spot":0.034637797},"886":{"spot":0.034641928},"887":{"spot":0.034646059},"888":{"spot":0.034650189},"889":{"spot":0.03465421},"890":{"spot":0.034658231},"891":{"spot":0.034662251},"892":{"spot":0.034666272},"893":{"spot":0.034670293},"894":{"spot":0.034674313},"895":{"spot":0.034678334},"896":{"spot":0.034682354},"897":{"spot":0.034686375},"898":{"spot":0.034690396},"899":{"spot":0.034694416},"900":{"spot":0.034698437}}"""

    type ZeroCouponRate = { duration: float; rate: float }

    let private spotList =
        json
        |> System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, float>>>
        |> Seq.map (fun kv -> (kv.Key, kv.Value))
        |> Seq.map (fun (k, v) ->
            { ZeroCouponRate.duration = float k
              ZeroCouponRate.rate = v["spot"] })
        |> Seq.toList

    let discountFactors = spotList |> List.map (fun s -> (1.0 + s.rate) ** -s.duration)
