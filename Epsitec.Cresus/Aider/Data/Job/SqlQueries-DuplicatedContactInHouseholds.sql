SELECT a.PERSON_ECHID, count (*)
FROM ECH_PERSON_WITH_HOUSEHOLDS a
where a.ECH_STATUS = 1
group by 
a.CONTACT,
a.PERSON_ECHID
having count (*) > 1