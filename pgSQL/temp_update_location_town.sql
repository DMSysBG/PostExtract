-- Function: temp_update_location_town(integer, character varying, integer)

-- DROP FUNCTION temp_update_location_town(integer, character varying, integer);

CREATE OR REPLACE FUNCTION temp_update_location_town(
    town_id integer,
    pattern character varying,
    province_id integer )
  RETURNS integer AS
$BODY$
BEGIN
   UPDATE n_template_location
      SET t_town_id = town_id
	, tl_name = trim(trim(trim(regexp_replace(tl_name, pattern, '', 'g')),','))
	, t_province_id = coalesce(t_province_id, province_id)
   WHERE tl_name ~* pattern
     AND t_town_id IS NULL
     AND coalesce(t_province_id, province_id) = province_id;
     
  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_update_location_town(integer, character varying, integer)
  OWNER TO postgres;
