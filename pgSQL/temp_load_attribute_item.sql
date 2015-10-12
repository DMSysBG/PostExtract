-- Function: temp_load_attribute_item()

-- DROP FUNCTION temp_load_attribute_item();
-- SELECT temp_load_attribute_item();

CREATE OR REPLACE FUNCTION temp_load_attribute_item()
  RETURNS integer AS
$BODY$ 
BEGIN
  -- Разпознава типа на атрибута
  UPDATE new_post_attribute
     SET n_category_attribute_type_id = CASE WHEN pa.n_category_attribute_type_id = 3 THEN -3		-- списъците нетрява да се добавят повторно
					     WHEN pa.n_category_attribute_type_id IS NULL THEN 1	-- всички неизвестни са от тип ст-ст
					     ELSE pa.n_category_attribute_type_id END
    FROM n_template_attribute ta
       , n_category_attribute pa
  WHERE ta.id = new_post_attribute.n_template_attribute_id
    AND pa.id = ta.n_category_attribute_id
    AND new_post_attribute.n_category_attribute_type_id IS NULL;

  -- Зарежда списъците като редове
  INSERT INTO new_post_attribute
  ( new_post_id, attribute_value, n_template_attribute_id, n_category_id, n_template_id, n_category_attribute_type_id)
  SELECT npa.new_post_id
       , regexp_split_to_table(npa.attribute_value, E'\\s+ ') AS attribute_value
       , npa.n_template_attribute_id
       , npa.n_category_id
       , npa.n_template_id
       , 3 AS n_category_attribute_type_id
  FROM new_post_attribute npa
  LEFT JOIN new_post_attribute vld
    ON vld.new_post_id = npa.new_post_id
   AND vld.n_category_attribute_type_id = 3
  WHERE npa.n_category_attribute_type_id = -3
    AND vld.id IS NULL;

   -- Добавя новите елементи на атребутите
  INSERT INTO n_template_attribute_item (n_template_attribute_id, tai_name)
  SELECT DISTINCT npa.n_template_attribute_id
       , npa.attribute_value AS tai_name
  FROM new_post_attribute npa
  LEFT JOIN n_template_attribute_item ta
    ON ta.n_template_attribute_id = npa.n_template_attribute_id
   AND ta.tai_name = npa.attribute_value
  WHERE npa.n_category_attribute_type_id IN (2, 3)
    AND ta.id IS NULL;

  -- Разпознава елементи на атребутите
  UPDATE new_post_attribute 
     SET n_template_attribute_item_id = ta.id
  FROM n_template_attribute_item ta
  WHERE ta.n_template_attribute_id = new_post_attribute.n_template_attribute_id
    AND ta.tai_name = new_post_attribute.attribute_value
    AND new_post_attribute.n_category_attribute_type_id IN (2, 3);

  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_load_attribute_item()
  OWNER TO postgres;
