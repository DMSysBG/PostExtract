-- Function: list_invalid_post()

-- DROP FUNCTION list_invalid_post();

CREATE OR REPLACE FUNCTION list_invalid_post()
  RETURNS TABLE(id integer, post_date timestamp with time zone, post_link text, msg_invalid character varying) AS
$BODY$ 
BEGIN
  -- SELECT * FROM list_invalid_post();
  RETURN QUERY
	SELECT p.id
	     , p.post_date
	     , p.post_link
	     , cast('Невалидна цена / post_price_type_id = ' || coalesce(p.post_price_type_id, 0) ||
		'; post_price = ' || coalesce(p.post_price, 0)
		as character varying) AS msg_invalid
	FROM post p 
	WHERE coalesce(p.post_price, 0) < 0.01
	  AND coalesce(p.post_price_type_id, 0) <> 1

	UNION
	SELECT p.id 
	     , p.post_date
	     , p.post_link
	     , cast('Невалидно местоположение / n_template_location.post_location = ' || tl.post_location
		as character varying) AS msg_invalid
	FROM post p
	INNER JOIN n_template_location tl
	   ON tl.id = p.n_template_location_id
	  AND (tl.n_template_province_id IS NULL OR tl.n_template_town_id IS NULL OR tl.n_template_region_id IS NULL);
	
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION list_invalid_post()
  OWNER TO postgres;
