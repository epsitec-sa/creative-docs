﻿CREATE VIEW SUBSCRIPTIONREFUSALS(
    ID,
    TYPE_ID,
    EM_ID,
    REFUSAL_TYPE,
    DISPLAY_NAME,
    DISPLAY_ADDRESS,
    ZIPCODE,
    HOUSEHOLD_ID,
    LEGALPERSON_ID)
AS
SELECT 
    CR_ID,
    CR_TYPE_ID,
    CR_EM_ID,
    U_LVGF32,
    U_LVGG32,
    U_LVGH32,
    U_LVGI32,
    U_LVGD32,
    U_LVGE32
FROM MUD_LVGC32;