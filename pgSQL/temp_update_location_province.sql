-- Function: temp_update_location_province(integer, character varying)

-- DROP FUNCTION temp_update_location_province(integer, character varying);

CREATE OR REPLACE FUNCTION temp_update_location_province(
    province_id integer,
    pattern character varying)
  RETURNS integer AS
$BODY$
BEGIN
   UPDATE n_template_location
      SET t_province_id = province_id
	, tl_name = trim(trim(trim(regexp_replace(tl_name, pattern, '', 'g')),','))
   WHERE tl_name ~* pattern
     AND t_province_id IS NULL;
     
  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_update_location_province(integer, character varying)
  OWNER TO postgres;
