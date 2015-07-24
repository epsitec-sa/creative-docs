﻿CREATE VIEW ECH_PERSON_WITH_HOUSEHOLDS(
    CONTACT,
    IS_HEAD,
    ECH_STATUS,
    AIDER_PERSON,
    AIDER_HOUSEHOLD,
    HOUSEOLD1_ADULT1_PERSON,
    HOUSEHOLD1_ADULT2_PERSON,
    HOUSEHOLD2_ADULT1_PERSON,
    HOUSEHOLD2_ADULT2_PERSON,
    PERSON_CRID,
    PERSON_ECHID,
    AIDER_HOUSEHOLD_CRID,
    HOUSEOLD1_ADULT1_CRID,
    HOUSEHOLD1_ADULT2_CRID,
    HOUSHOLD2_ADULT1_CRID,
    HOUSEHOLD2_ADULT2_CRID
    )
AS
SELECT 
c.U_LVACE,
c.U_LVA9E,
p.U_LVAS2,
ap.U_LVAI4,
ah.U_LVAGE,
aph1a1.U_LVAI4,
aph1a2.U_LVAI4,
aph2a1.U_LVAI4,
aph2a2.U_LVAI4,
ap.CR_ID,
p.U_LVA1,
ah.CR_ID,
h1.U_LVAH,
h1.U_LVAI,
h2.U_LVAH,
h2.U_LVAI
FROM 
MUD_LVA p 
left join MUD_LVAF ap on ap.U_LVAU1 = p.CR_ID
left join MUD_LVARD c on c.U_LVA5E = ap.CR_ID
left join MUD_LVAI2 ah on c.U_LVA6E = ah.CR_ID
left join MUD_LVAG h1 on p.U_LVAG2 = h1.CR_ID
left join MUD_LVAG h2 on p.U_LVAH2 = h2.CR_ID
left join MUD_LVA ph1a1 on h1.U_LVAH = ph1a1.CR_ID 
left join MUD_LVA ph1a2 on h1.U_LVAI = ph1a2.CR_ID
left join MUD_LVA ph2a1 on h2.U_LVAH = ph2a1.CR_ID
left join MUD_LVA ph2a2 on h2.U_LVAI = ph2a2.CR_ID
left join MUD_LVAF aph1a1 on aph1a1.U_LVAU1 = ph1a1.CR_ID 
left join MUD_LVAF aph1a2 on aph1a2.U_LVAU1 = ph1a2.CR_ID
left join MUD_LVAF aph2a1 on aph2a1.U_LVAU1 = ph2a1.CR_ID
left join MUD_LVAF aph2a2 on aph2a2.U_LVAU1 = ph2a2.CR_ID
where
c.U_LVA4E = 1 and
p.U_LVA1 is not null;