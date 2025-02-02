import React from 'react'

export default function ListGym() {
    const [database, setDatabase] = useState([])
    console.log(database)
  
    useEffect(() => {
        GetFetch()
    }, [])

    function GetFetch(){
        fetch()
        .then((res)=>res.json())
        .then((data)=>{setDatabase(data)})
      }



  return (
    <div>ListGym</div>
  )
}
