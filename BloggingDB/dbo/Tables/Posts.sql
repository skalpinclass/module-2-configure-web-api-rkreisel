CREATE TABLE [dbo].[Posts] (
    [BlogPostId] INT            IDENTITY (1, 1) NOT NULL,
    [Title]      NVARCHAR (MAX) NULL,
    [Content]    NVARCHAR (MAX) NULL,
    [BlogId]     INT            NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED ([BlogPostId] ASC),
    CONSTRAINT [FK_Posts_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [dbo].[Blogs] ([BlogId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Posts_BlogId]
    ON [dbo].[Posts]([BlogId] ASC);

