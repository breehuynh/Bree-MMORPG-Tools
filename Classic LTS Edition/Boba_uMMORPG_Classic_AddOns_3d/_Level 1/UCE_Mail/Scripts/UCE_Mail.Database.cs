// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using System;

#if _MYSQL && _SERVER
using MySql.Data;
using MySql.Data.MySqlClient;
#elif _SQLITE && _SERVER
using SQLite;
#endif

// DATABASE

public partial class Database
{

#if _SQLITE && _SERVER
 	// -----------------------------------------------------------------------------------
    // Mail
    // -----------------------------------------------------------------------------------
    public class mail
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public int id { get; set; }
        public string messageFrom { get; set; }
        public string messageTo { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public long sent { get; set; }
        public long expires { get; set; }
        public int read { get; set; }
        public int deleted { get; set; }
        public string item { get; set; }
    }
#endif
    
    // -----------------------------------------------------------------------------------
    // Connect_Mail
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_Mail()
    {
#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"
                            CREATE TABLE IF NOT EXISTS mail(
							id INTEGER(16) NOT NULL AUTO_INCREMENT,
							messageFrom VARCHAR(32) NOT NULL,
							messageTo VARCHAR(32) NOT NULL,
							subject VARCHAR(32) NOT NULL,
							body VARCHAR(512) NOT NULL,
							sent BIGINT(16) NOT NULL,
							expires BIGINT(16) NOT NULL,
							`read` BIGINT(16) NOT NULL,
							`deleted` BIGINT(16) NOT NULL,
							`item` VARCHAR(32) NOT NULL,
                            PRIMARY KEY(id)
                            ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<mail>();
#endif
    }

#if _MYSQL && _SERVER
    // -----------------------------------------------------------------------------------
    // MySql Mail_BuildMessageFromDBRow
    // -----------------------------------------------------------------------------------
    public MailMessage Mail_BuildMessageFromDBRow(List<object> row)
    {
        MailMessage message = new MailMessage();

        int colNum = 0;

		message.id		= (int)row[colNum++];
		message.from	= (string)row[colNum++];
		message.to		= (string)row[colNum++];
		message.subject = (string)row[colNum++];
		message.body	= (string)row[colNum++];
		message.sent	= (long)row[colNum++];
		message.expires = (long)row[colNum++];
		message.read	= (long)row[colNum++];
		message.deleted = (long)row[colNum++];

		string name = (string)row[colNum++];
		if (ScriptableItem.dict.TryGetValue(name.GetStableHashCode(), out ScriptableItem itemData))
        	message.item = itemData;

        return message;
    }
#endif

#if _SQLITE && _SERVER
    // -----------------------------------------------------------------------------------
    // Sqlite Mail_BuildMessageFromDBRow
    // -----------------------------------------------------------------------------------
    public MailMessage Mail_BuildMessageFromDBRow(mail row)
    {
        MailMessage message = new MailMessage();

        message.id = row.id;
        message.from = row.messageFrom;
        message.to = row.messageTo;
        message.subject = row.subject;
        message.body = row.body;
        message.sent = row.sent;
        message.expires = row.expires;
        message.read = row.read;
        message.deleted = row.deleted;

        string item = row.item;
        if (ScriptableItem.dict.TryGetValue(item.GetStableHashCode(), out ScriptableItem itemData))
            message.item = itemData;

        return message;
    }

#endif

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_Mail(Player player)
    {
#if _SERVER
#if _MYSQL
		var table = ExecuteReaderMySql("SELECT * FROM mail WHERE messageTo=@character AND deleted=0 AND expires > @expires ORDER BY sent", new MySqlParameter("@character", player.name), new MySqlParameter("@expires", Epoch.Current()));
#elif _SQLITE
        var table = connection.Query<mail>("SELECT * FROM mail WHERE messageTo=? AND deleted=0 AND expires > " + Epoch.Current() + " ORDER BY sent", player.name);
#endif
        foreach (var row in table)
        {
            MailMessage message = Mail_BuildMessageFromDBRow(row);
            player.mailMessages.Add(message);
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public List<MailSearch> Mail_SearchForCharacter(string name, string selfPlayer)
    {
        List<MailSearch> result = new List<MailSearch>();

#if _MYSQL && _SERVER
		var table = ExecuteReaderMySql(@"SELECT `name` , level FROM `characters` WHERE name=@search", new MySqlParameter("@search", name));

		foreach (var row in table) {
			MailSearch res = new MailSearch();
			res.name = (string)row[0];
			res.level = Convert.ToInt32((int)row[1]);
			res.guild = "";

			result.Add(res);
		}

#elif _SQLITE && _SERVER
        /**
		 * Order by here is setup in such a way that:
		 *		exact matches appear first
		 *		followed by names where the search string is closer to the front of the name
		 */
        var table = connection.Query<characters>(string.Concat(@"SELECT `name`, level
                                                            FROM characters
                                                            LEFT JOIN character_guild
                                                            ON character=name
                                                            WHERE name
                                                                LIKE '%' || ? || '%' " +
                                                                "AND name <> ? " +
                                                                "ORDER BY CASE " +
                                                                "WHEN name=? " +
                                                                "THEN 0 " +
                                                                "   ELSE INSTR(LOWER(name), LOWER(?)) " +
                                                            "END, name LIMIT 30", name, selfPlayer, name, name));

        foreach (var row in table)
        {
            MailSearch res = new MailSearch();
            res.name = row.name;
            res.level = row.level;
            res.guild = "";

            result.Add(res);
        }

#endif

        return result;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void Mail_CreateMessage(string from, string to, string subject, string body, string itemName, long expiration = 0)
    {
        long sent = Epoch.Current();
        long expires = 0;
        
        if (expiration > 0)
            expires = sent + expiration;

        if (itemName == null) itemName = "";

#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"INSERT INTO mail (
							messageFrom, messageTo, subject, body, sent, `expires`, `read`, `deleted`, `item`
						) VALUES (
							@from, @to, @subject, @body, @sent, @expires, 0, 0, @item
						)",
						new MySqlParameter("@from", from),
						new MySqlParameter("@to", to),
						new MySqlParameter("@subject", subject),
						new MySqlParameter("@body", body),
						new MySqlParameter("@sent", sent),
						new MySqlParameter("@expires", expires),
						new MySqlParameter("@item", itemName )
						);
#elif _SQLITE && _SERVER
        connection.Insert(new mail
        {
            messageFrom = from,
            messageTo = to,
            subject = subject,
            body = body,
            sent = sent,
            expires = expires,
            item = itemName
        });
#endif
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void Mail_UpdateMessage(MailMessage message)
    {
        string itemName = "";
        if (message.item != null)
            itemName = message.item.name;

#if _MYSQL && _SERVER
		ExecuteNonQueryMySql(@"UPDATE mail SET
							`read`=@read,
							deleted=@deleted,
							item=@item
						WHERE id=@id",
						new MySqlParameter("@read", message.read),
						new MySqlParameter("@deleted", message.deleted),
						new MySqlParameter("@item", itemName),
						new MySqlParameter("@id", message.id));

#elif _SQLITE && _SERVER
        connection.Execute(@"UPDATE mail SET read=?, deleted=?, item=? WHERE id=?", message.read, message.deleted, itemName, message.id);
#endif
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public MailMessage Mail_MessageById(long id)
    {
    	MailMessage message = new MailMessage();
#if _SERVER
#if _MYSQL
		var table = ExecuteReaderMySql("SELECT * FROM mail WHERE id=@id", new MySqlParameter("@id", id));
#elif _SQLITE
        var table = connection.Query<mail>("SELECT * FROM mail WHERE id=?", id);
#endif
        if (table.Count == 1)
        {
            message = Mail_BuildMessageFromDBRow(table[0]);
        }
#endif
		return message;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public List<MailMessage> Mail_CheckForNewMessages(long maxID)
    {
        List<MailMessage> result = new List<MailMessage>();
#if _SERVER
#if _MYSQL
		var table = ExecuteReaderMySql("SELECT * FROM mail WHERE id > @maxid AND deleted=0 AND expires > @expires ORDER BY sent", new MySqlParameter("@maxid", maxID), new MySqlParameter("@expires", Epoch.Current()));
#elif _SQLITE
        var table = connection.Query<mail>("SELECT * FROM mail WHERE id > " + maxID + " AND deleted=0 AND expires > " + Epoch.Current() + " ORDER BY sent");
#endif
        foreach (var row in table)
        {
            MailMessage message = Mail_BuildMessageFromDBRow(row);
            result.Add(message);
        }
#endif
        return result;
    }

    // -----------------------------------------------------------------------------------
}
