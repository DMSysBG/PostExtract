using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using DMSys.Systems;

namespace PostExtract
{
    public class DBExtract : IDisposable
    {
        private NpgsqlConnection _Connection = null;

        public DBExtract()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["posts"].ConnectionString;

            _Connection = new NpgsqlConnection(connectionString);
            _Connection.Open();
        }
                
        public void Dispose()
        {
            if (_Connection != null)
            {
                _Connection.Close();
                _Connection.Dispose();
                _Connection = null;
            }
        }

        public PExtractTemplate GetPETemplate(int sourceId)
        {
            PExtractTemplate peTemplate = null;

            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                command.CommandText = String.Format(
@"SELECT s.id AS n_source_id
       , s.s_link
       , s.n_site_id
       , s.n_category_id
       , t.id AS n_template_id
       , t.t_name
       , t.xp_list
       , t.xp_list_link
       , t.xp_list_image
       , t.xp_post
       , t.xp_post_title
       , t.xp_post_text
       , t.xp_post_images
       , t.xp_post_attributes
       , t.sql_post_attributes
       , t.xp_post_price
       , t.xp_post_location
       , t.xp_post_date
       , t.xp_post_posted
FROM n_source s
INNER JOIN n_template t ON t.id = s.n_template_id
WHERE s.id = {0} ", sourceId);
                using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command))
                {
                    using (DataTable dtSource = new DataTable())
                    {
                        dataAdapter.Fill(dtSource);
                        if (dtSource.Rows.Count > 0)
                        {
                            peTemplate = CreatePETemplate(dtSource.Rows[0]);
                        }
                    }
                }
            }
            return peTemplate;
        }
        
        public List<PExtractTemplate> GetPETemplates()
        {
            List<PExtractTemplate> peTemplates = new List<PExtractTemplate>();

            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                command.CommandText =
@"SELECT s.id AS n_source_id
       , s.s_link
       , s.n_site_id
       , s.n_category_id
       , t.id AS n_template_id
       , t.t_name
       , t.xp_list
       , t.xp_list_link
       , t.xp_list_image
       , t.xp_post
       , t.xp_post_title
       , t.xp_post_text
       , t.xp_post_images
       , t.xp_post_attributes
       , t.sql_post_attributes
       , t.xp_post_price
       , t.xp_post_location
       , t.xp_post_date
       , t.xp_post_posted
FROM n_source s
INNER JOIN n_template t ON t.id = s.n_template_id
WHERE s.is_active = 1 ";
                using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command))
                {
                    using (DataTable dtSource = new DataTable())
                    {
                        dataAdapter.Fill(dtSource);
                        foreach (DataRow drSource in dtSource.Rows)
                        {
                            peTemplates.Add(CreatePETemplate(drSource));                            
                        }
                    }
                }
            }
            return peTemplates;
        }

        private PExtractTemplate CreatePETemplate(DataRow drSource)
        {
            PExtractTemplate peTemplate = new PExtractTemplate()
            {
                SourceId = TryParse.ToInt32(drSource["n_source_id"]),
                CategoryId = TryParse.ToInt32(drSource["n_category_id"]),
                TemplateId = TryParse.ToInt32(drSource["n_template_id"]),
                SiteUrl = TryParse.ToString(drSource["s_link"]),
                XPList = TryParse.ToString(drSource["xp_list"]),
                XPLink = TryParse.ToString(drSource["xp_list_link"]),
                XPImage = TryParse.ToString(drSource["xp_list_image"]),
                XPContent = TryParse.ToString(drSource["xp_post"]),
                XPTitle = TryParse.ToString(drSource["xp_post_title"]),
                XPText = TryParse.ToString(drSource["xp_post_text"]),
                XPImages = TryParse.ToString(drSource["xp_post_images"]),
                XPAttributes = TryParse.ToString(drSource["xp_post_attributes"]),
                SQLAttributes = TryParse.ToString(drSource["sql_post_attributes"]),
                XPPrice = TryParse.ToString(drSource["xp_post_price"]),
                XPLocation = TryParse.ToString(drSource["xp_post_location"]),
                XPDate = TryParse.ToString(drSource["xp_post_date"]),
                XPPosted = TryParse.ToString(drSource["xp_post_posted"])
            };
            return peTemplate;
        }

        public void EmptyNewPost()
        {
            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                command.CommandText =
@"DELETE FROM new_post_image;
  DELETE FROM new_post_attribute;
  DELETE FROM new_post;
  ALTER SEQUENCE new_post_image_id_seq RESTART WITH 1;
  ALTER SEQUENCE new_post_attribute_id_seq RESTART WITH 1;
  ALTER SEQUENCE new_post_id_seq RESTART WITH 1; ";

                command.ExecuteNonQuery();
            }
        }

        public int AddNewPost(int sourceId, int categoryId, int templateId, string link, string image)
        {
            int rowId = 0;
            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                // Записва данните
                command.CommandText = String.Format(
@"INSERT INTO new_post ( n_source_id, n_category_id, n_template_id, post_link, post_image)
  VALUES ( {0}, {1}, {2}, '{3}', '{4}')
  RETURNING id; ", sourceId, categoryId, templateId, link, image);

                rowId = TryParse.ToInt32( command.ExecuteScalar());
            }
            return rowId;
        }

        public DataTable GetNewPost()
        {
            DataTable dtPosts = new DataTable();
            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                command.CommandText = "SELECT id, post_link FROM new_post";
                using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command))
                {
                    dataAdapter.Fill(dtPosts);
                }
            }
            return dtPosts;
        }

        /// <summary>
        /// Записва публикацията
        /// </summary>
        public void SaveNewPost(int postId, int categoryId, int templateId, PPost pPost)
        {
            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                command.CommandText = String.Format(
@"UPDATE new_post
     SET post_title = '{1}'
       , post_text = '{2}'
       , post_price = '{3}'
       , post_location = '{4}'
       , post_date = '{5}'
       , post_posted = '{6}'
  WHERE id = {0}", postId
                 , NpgsqlPString(pPost.Title)
                 , NpgsqlPString(pPost.Text)
                 , NpgsqlPString(pPost.Price)
                 , NpgsqlPString(pPost.Location)
                 , NpgsqlPString(pPost.Date)
                 , NpgsqlPString(pPost.Posted));

                command.ExecuteNonQuery();

                foreach (string imgSrc in pPost.Images)
                {
                    command.CommandText = String.Format(
@"INSERT INTO new_post_image ( new_post_id, img_src )
VALUES ( {0}, '{1}' ) ", postId, NpgsqlPString(imgSrc));

                    command.ExecuteNonQuery();
                }

                foreach (string attribute in pPost.Attributes)
                {
                    command.CommandText = String.Format(
@"INSERT INTO new_post_attribute ( new_post_id, attribute_value, n_category_id, n_template_id )
VALUES ( {0}, '{1}', {2}, {3}) ", postId, NpgsqlPString(attribute), categoryId, templateId);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Прехвърля новите публикации
        /// </summary>
        public void TransferNewPost()
        {
            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT transfer_new_post()";
                command.ExecuteScalar();
            }
        }
        
        /// <summary>
        /// Премахва дублираните
        /// </summary>
        public void RemovesDuplicate()
        {
            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                command.CommandText =
@"DELETE FROM new_post
WHERE id in ( SELECT np.id FROM new_post np
		      INNER JOIN post p ON p.post_link = np.post_link ) ";
                command.ExecuteNonQuery();

                command.CommandText =
@"DELETE FROM new_post_attribute
WHERE new_post_id NOT IN (SELECT id FROM new_post) ";
                command.ExecuteNonQuery();

                command.CommandText =
@"DELETE FROM new_post_image
WHERE new_post_id NOT IN (SELECT id FROM new_post) ";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Валидира публикациите
        /// </summary>
        public void ValidatePublications()
        {
            using (NpgsqlCommand command = _Connection.CreateCommand())
            {
                // локации
                command.CommandText = "SELECT temp_load_location();";
                command.ExecuteScalar();

                // публикатори
                command.CommandText = "SELECT temp_load_site_posted();";
                command.ExecuteScalar();

                // местоположения
                command.CommandText = "SELECT temp_repair_location_item();";
                command.ExecuteScalar();

                // атрибути
                command.CommandText = "SELECT temp_load_attribute_item();";
                command.ExecuteScalar();

                // Записва в лога публ. с неразпоснати атрибути
                command.CommandText =
@"INSERT INTO sys_exception (n_source_id, post_link, ex_message, stack_trace, ex_date)
 SELECT DISTINCT np.n_source_id
	  , np.post_link
	  , 'attribute: '''||npa.attribute_value||'''' AS stack_trace
	  , 'Невалиден атрибут' AS ex_message
      , now() AS ex_date
 FROM new_post np
 LEFT JOIN new_post_attribute npa ON npa.new_post_id = np.id
 LEFT JOIN n_template_attribute ta ON ta.id = npa.n_template_attribute_id
 LEFT JOIN n_category_attribute pa ON pa.id = ta.n_category_attribute_id
 WHERE pa.id IS NULL ";
                command.ExecuteNonQuery();

                // Премахва атрибутите без публикация
                command.CommandText =
@"DELETE FROM new_post_attribute
WHERE new_post_id NOT IN (SELECT id FROM new_post) ";
                command.ExecuteNonQuery();

                // Премахва снимките без публикация
                command.CommandText =
@"DELETE FROM new_post_image
WHERE new_post_id NOT IN (SELECT id FROM new_post) ";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Изпълнява подадения SQL
        /// </summary>
        public void ExecuteNonQuery(string commandText)
        {
            if (!String.IsNullOrWhiteSpace(commandText))
            {
                using (NpgsqlCommand command = _Connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        private string NpgsqlPString(string value)
        {
            return value.Replace("'", "''");
        }
    }
}
