SELECT tl.id
     --, tl.n_template_id
     , tl.post_location
     , tl.n_template_province_id
     , tp.tp_name
     , tl.n_template_town_id
     , tt.tt_name
     , tl.n_template_region_id
     , tr.tr_name
FROM n_template_location tl
LEFT JOIN n_template_province tp ON tp.id = tl.n_template_province_id
LEFT JOIN n_template_town tt ON tt.id = tl.n_template_town_id
LEFT JOIN n_template_region tr ON tr.id = tl.n_template_region_id

-- WHERE tl.post_location LIKE 'м-т %'


/*
UPDATE n_template_location
   SET n_template_province_id = NULL
     , n_template_town_id = NULL
     , n_template_region_id = NULL
-- WHERE n_template_town_id = 375

UPDATE n_template_location
   SET n_template_region_id = NULL
WHERE n_template_region_id = 0

*/
