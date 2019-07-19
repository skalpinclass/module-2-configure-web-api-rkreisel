/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *
  FROM [Blogging].[dbo].[Blogs] b
  join Blogging.dbo.Posts p on p.BlogId = b.BlogId
  order by b.BlogId, p.BlogPostId