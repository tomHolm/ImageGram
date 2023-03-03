function deleteComment(commentId, postId) {
    var collection = getContext().getCollection();
    collection.deleteDocument(
        `${collection.getAltLink()}/docs/${commentId}`,
        {},
        function (err) {
            if (err) {
                throw err;
            }

            setResult(true);
            collection.readDocument(
                `${collection.getAltLink()}/docs/${postId}`,
                function (err, post) {
                    if (err) {
                        throw err;
                    }

                    if (post && post.comments) {
                        if (post.comments.filter(c => c.id === commentId).length > 0) {
                            collection.queryDocuments(
                                collection.getSelfLink(),
                                'SELECT * FROM c WHERE c.postId = "' + postId + '" AND c.type = "comment" ORDER BY c.timestamp DESC',
                                { pageSize: 2 },
                                function (err, resources) {
                                    if (err) {
                                        throw err;
                                    }

                                    post.commentsCount--;
                                    post.comments = [];

                                    if (resources.length > 0) {
                                        resources.forEach(comItem => post.comments.push(comItem));
                                    }

                                    collection.replaceDocument(
                                        post._self,
                                        post
                                    );
                                }
                            );
                        }
                    }
                }
            )
        }
    );
}

function setResult(value) {
    getContext().getResponse().setBody({ success: value });
}