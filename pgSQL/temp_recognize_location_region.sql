-- Function: temp_recognize_location_region()

-- DROP FUNCTION temp_recognize_location_region();

CREATE OR REPLACE FUNCTION temp_recognize_location_region()
  RETURNS integer AS
$BODY$
BEGIN
   -- Разпознава районите към градовете
   UPDATE n_template_location
      SET n_template_region_id = tr.id
      
   FROM n_template_location_item tli
      , n_template_region tr
      
   WHERE n_template_location.n_template_region_id IS NULL			-- Неразпознати райони
     AND tli.n_template_location_id = n_template_location.id
     AND tli.t_location_type_id IN (5, 6, 7)
										-- Търси съответствия
     AND tr.n_template_town_id = n_template_location.n_template_town_id
     AND tr.t_location_type_id = tli.t_location_type_id
     AND lower(tr.tr_name) = lower(tli.t_location_name);
     
  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_recognize_location_region()
  OWNER TO postgres;
