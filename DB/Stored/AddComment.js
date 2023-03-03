function createComment(postId, comment) {
    var collection = getContext().getCollection();
    collection.readDocument(
        `${collection.getAltLink()}/docs/${postId}`,
        function (err, post) {
            if (err) {
                throw err;
            }
    
            post.commentsCount++;
            post.comments.unshift(comment);
            if (post.comments.length > 2) {
                post.comments.pop();
            }

            collection.replaceDocument(
                post._self,
                post,
                function (err) {
                    if (err) {
                        throw err;
                    }

                    comment.postId = postId;
                    collection.createDocument(
                        collection.getSelfLink(),
                        comment
                    );
                }
            );
        }
    );
    getContext().getResponse().setBody(comment);
}