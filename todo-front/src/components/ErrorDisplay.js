/*
Error boundary
 */
function ErrorDisplay({title, status, errors}) {

    const displayErrorItems = (errorList) => (<ul>
        {Object.entries(errorList).map(([key, descList]) =>
                descList.map(desc => <li>{key}: {desc}</li>))}
    </ul>);

    /*
    Change message style according to status
     */
    const evaluateErrorStyle = (code) => {
        if (code < 400) {
            return "is-success";
        } else if (code < 500) {
            return "is-warning";
        } else {
            return "is-danger";
        }
    }

    return (<article className={`message ${evaluateErrorStyle(status)}`}>
        <div className="message-header">
            <p>{status}: {title}</p>
        </div>
        <div className="message-body">
            {displayErrorItems(errors)}
        </div>
    </article>);
}

export default ErrorDisplay;
